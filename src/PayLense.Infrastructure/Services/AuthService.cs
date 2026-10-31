using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PayLense.Application.Common;
using PayLense.Application.DTOs.Auth;
using PayLense.Application.Interfaces;
using PayLense.Domain.Entities;
using PayLense.Domain.Enums;
using PayLense.Domain.Exceptions;
using PayLense.Infrastructure.Identity;
using PayLense.Infrastructure.Persistence;

namespace PayLense.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly AppDbContext _db;
    private readonly ITokenService _tokens;
    private readonly IOtpService _otp;
    private readonly IOtpSender _sender;
    private readonly ICardEncryptor _encryptor;
    private readonly JwtSettings _jwt;

    public AuthService(
        UserManager<ApplicationUser> users,
        AppDbContext db,
        ITokenService tokens,
        IOtpService otp,
        IOtpSender sender,
        ICardEncryptor encryptor,
        IOptions<JwtSettings> jwt)
    {
        _users = users;
        _db = db;
        _tokens = tokens;
        _otp = otp;
        _sender = sender;
        _encryptor = encryptor;
        _jwt = jwt.Value;
    }

    // ------------------------------------------------------------ Signup
    public async Task<MessageResponse> SignupAsync(SignupRequest r)
    {
        var cardNumber = CardValidator.Normalize(r.CardNumber);
        if (!CardValidator.IsValidNumber(cardNumber))
            throw Validation("cardNumber", "Invalid card number.");
        if (CardValidator.IsExpired(r.ExpiryMonth, r.ExpiryYear))
            throw Validation("expiryDate", "Card is expired.");

        var email = r.Email.Trim();
        var phone = r.PhoneNumber.Trim();
        var username = r.Username.Trim();

        if (await _users.FindByEmailAsync(email) != null)
            throw Validation("email", "Email is already registered.");
        if (await _users.FindByNameAsync(username) != null)
            throw Validation("username", "Username is already taken.");
        if (await _users.Users.AnyAsync(u => u.PhoneNumber == phone))
            throw Validation("phoneNumber", "Phone number is already registered.");

        ApplicationUser user;
        await using (var tx = await _db.Database.BeginTransactionAsync())
        {
            user = new ApplicationUser { UserName = username, Email = email, PhoneNumber = phone };

            var result = await _users.CreateAsync(user, r.Password);
            if (!result.Succeeded) throw IdentityFailure(result);

            _db.BankCards.Add(new BankCard
            {
                UserId = user.Id,
                CardNumberEncrypted = _encryptor.Encrypt(cardNumber),
                Last4Digits = cardNumber[^4..],
                ExpiryMonth = r.ExpiryMonth,
                ExpiryYear = r.ExpiryYear,
                IsDefault = true
            });
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        var emailCode = await _otp.GenerateAsync(user.Id, OtpPurpose.EmailConfirmation);
        var phoneCode = await _otp.GenerateAsync(user.Id, OtpPurpose.PhoneConfirmation);
        await _sender.SendEmailAsync(email, emailCode, OtpPurpose.EmailConfirmation);
        await _sender.SendSmsAsync(phone, phoneCode, OtpPurpose.PhoneConfirmation);

        return new MessageResponse
        {
            Message = "Account created. Please confirm your email and phone number using the codes we sent."
        };
    }

    // ------------------------------------------------- Confirm email / phone
    public async Task<MessageResponse> VerifyEmailAsync(VerifyEmailRequest r)
    {
        var user = await _users.FindByEmailAsync(r.Email.Trim())
            ?? throw new AppException("Invalid or expired code.");

        if (user.EmailConfirmed) return new MessageResponse { Message = "Email already confirmed." };

        if (!await _otp.VerifyAsync(user.Id, OtpPurpose.EmailConfirmation, r.Otp))
            throw new AppException("Invalid or expired code.");

        user.EmailConfirmed = true;
        await _users.UpdateAsync(user);
        return new MessageResponse { Message = "Email confirmed." };
    }

    public async Task<MessageResponse> VerifyPhoneAsync(VerifyPhoneRequest r)
    {
        var phone = r.PhoneNumber.Trim();
        var user = await _users.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone)
            ?? throw new AppException("Invalid or expired code.");

        if (user.PhoneNumberConfirmed) return new MessageResponse { Message = "Phone already confirmed." };

        if (!await _otp.VerifyAsync(user.Id, OtpPurpose.PhoneConfirmation, r.Otp))
            throw new AppException("Invalid or expired code.");

        user.PhoneNumberConfirmed = true;
        await _users.UpdateAsync(user);
        return new MessageResponse { Message = "Phone number confirmed." };
    }

    public async Task<MessageResponse> ResendOtpAsync(ResendOtpRequest r)
    {
        // رسالة موحدة عشان محدش يعرف مين مسجل ومين لأ
        var generic = new MessageResponse { Message = "If the account exists, a new code has been sent." };

        var identifier = r.Identifier.Trim();
        var user = r.Purpose == OtpPurpose.PhoneConfirmation
            ? await _users.Users.FirstOrDefaultAsync(u => u.PhoneNumber == identifier)
            : await FindByIdentifierAsync(identifier);

        if (user == null) return generic;

        switch (r.Purpose)
        {
            case OtpPurpose.EmailConfirmation when !user.EmailConfirmed:
                await _sender.SendEmailAsync(user.Email!,
                    await _otp.GenerateAsync(user.Id, r.Purpose), r.Purpose);
                break;

            case OtpPurpose.PhoneConfirmation when !user.PhoneNumberConfirmed:
                await _sender.SendSmsAsync(user.PhoneNumber!,
                    await _otp.GenerateAsync(user.Id, r.Purpose), r.Purpose);
                break;

            case OtpPurpose.LoginTwoFactor when user.EmailConfirmed && user.PhoneNumberConfirmed:
                await _sender.SendEmailAsync(user.Email!,
                    await _otp.GenerateAsync(user.Id, r.Purpose), r.Purpose);
                break;
        }

        return generic;
    }

    // ------------------------------------------------------------ Signin
    public async Task<SigninResponse> SigninAsync(SigninRequest r)
    {
        var user = await FindByIdentifierAsync(r.Identifier)
            ?? throw new AppException("Invalid credentials.", 401);

        if (await _users.IsLockedOutAsync(user))
            throw new AppException("Account is temporarily locked. Try again later.", 429);

        if (!await _users.CheckPasswordAsync(user, r.Password))
        {
            await _users.AccessFailedAsync(user);
            throw new AppException("Invalid credentials.", 401);
        }
        await _users.ResetAccessFailedCountAsync(user);

        if (!user.EmailConfirmed || !user.PhoneNumberConfirmed)
            throw new AppException("Please confirm your email and phone number first.", 403);

        // جهاز موثوق => نتخطى الـ 2FA
        var trusted = await _db.TrustedDevices.FirstOrDefaultAsync(d =>
            d.UserId == user.Id && d.DeviceIdentifier == r.DeviceId && !d.IsRevoked);

        if (trusted != null)
        {
            trusted.LastUsedAt = DateTime.UtcNow;
            return new SigninResponse
            {
                RequiresTwoFactor = false,
                Auth = await IssueTokensAsync(user, r.DeviceId)
            };
        }

        var code = await _otp.GenerateAsync(user.Id, OtpPurpose.LoginTwoFactor);
        await _sender.SendEmailAsync(user.Email!, code, OtpPurpose.LoginTwoFactor);

        return new SigninResponse
        {
            RequiresTwoFactor = true,
            Message = "A verification code was sent to your email."
        };
    }

    public async Task<AuthResponse> VerifyTwoFactorAsync(VerifyTwoFactorRequest r)
    {
        var invalid = new AppException("Invalid or expired code.");

        var user = await FindByIdentifierAsync(r.Identifier) ?? throw invalid;
        if (await _users.IsLockedOutAsync(user)) throw invalid;

        if (!await _otp.VerifyAsync(user.Id, OtpPurpose.LoginTwoFactor, r.Otp))
            throw invalid;

        if (r.TrustDevice)
            await TrustDeviceAsync(user.Id, r.DeviceId, r.DeviceName);

        return await IssueTokensAsync(user, r.DeviceId);
    }

    // ------------------------------------------------------ Refresh / Logout
    public async Task<AuthResponse> RefreshAsync(RefreshRequest r)
    {
        var invalid = new AppException("Invalid refresh token.", 401);

        var hash = _tokens.Hash(r.RefreshToken);
        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash)
            ?? throw invalid;

        if (stored.DeviceId != r.DeviceId) throw invalid;

        // token اتستخدم قبل كده => احتمال اتسرق، نبطّل كل tokens الجهاز ده
        if (stored.RevokedAt != null)
        {
            var siblings = await _db.RefreshTokens
                .Where(t => t.UserId == stored.UserId && t.DeviceId == stored.DeviceId && t.RevokedAt == null)
                .ToListAsync();
            foreach (var s in siblings) s.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            throw invalid;
        }

        if (stored.ExpiresAt <= DateTime.UtcNow) throw invalid;

        var user = await _users.FindByIdAsync(stored.UserId) ?? throw invalid;
        return await IssueTokensAsync(user, r.DeviceId, replacing: stored);
    }

    public async Task<MessageResponse> LogoutAsync(LogoutRequest r)
    {
        var hash = _tokens.Hash(r.RefreshToken);
        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);

        if (stored != null && stored.RevokedAt == null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return new MessageResponse { Message = "Logged out." };
    }

    // ----------------------------------------------------------- Helpers
    private async Task<AuthResponse> IssueTokensAsync(
        ApplicationUser user, string deviceId, RefreshToken? replacing = null)
    {
        var (access, expiresAt) = _tokens.CreateAccessToken(
            user.Id, user.UserName!, user.Email!, deviceId);
        var (plain, hash) = _tokens.CreateRefreshToken();

        if (replacing != null)
        {
            replacing.RevokedAt = DateTime.UtcNow;
            replacing.ReplacedByTokenHash = hash;
        }

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = hash,
            DeviceId = deviceId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays)
        });
        await _db.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = access,
            RefreshToken = plain,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            }
        };
    }

    private async Task TrustDeviceAsync(string userId, string deviceId, string? deviceName)
    {
        var device = await _db.TrustedDevices
            .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceIdentifier == deviceId);

        if (device == null)
        {
            _db.TrustedDevices.Add(new TrustedDevice
            {
                UserId = userId,
                DeviceIdentifier = deviceId,
                DeviceName = deviceName ?? "Unknown device"
            });
        }
        else
        {
            device.IsRevoked = false;
            device.TrustedAt = DateTime.UtcNow;
            device.LastUsedAt = DateTime.UtcNow;
            if (deviceName != null) device.DeviceName = deviceName;
        }
        await _db.SaveChangesAsync();
    }

    private Task<ApplicationUser?> FindByIdentifierAsync(string identifier)
    {
        identifier = identifier.Trim();
        return identifier.Contains('@')
            ? _users.FindByEmailAsync(identifier)
            : _users.FindByNameAsync(identifier);
    }

    private static AppException Validation(string field, string message) =>
        new("Validation failed.", 400, new Dictionary<string, string[]> { [field] = new[] { message } });

    private static AppException IdentityFailure(IdentityResult result)
    {
        var errors = result.Errors
            .GroupBy(e =>
                e.Code.StartsWith("Password") ? "password" :
                e.Code.Contains("UserName") ? "username" :
                e.Code.Contains("Email") ? "email" : "general")
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        return new AppException("Registration failed.", 400, errors);
    }
}
