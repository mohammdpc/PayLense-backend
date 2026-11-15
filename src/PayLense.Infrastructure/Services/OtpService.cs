using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PayLense.Application.Interfaces;
using PayLense.Domain.Entities;
using PayLense.Domain.Enums;
using PayLense.Domain.Exceptions;
using PayLense.Infrastructure.Persistence;

namespace PayLense.Infrastructure.Services;

public class OtpService : IOtpService
{
    private const int ExpiryMinutes = 5;
    private const int MaxAttempts = 5;
    private const int ResendCooldownSeconds = 60;

    private readonly AppDbContext _db;

    public OtpService(AppDbContext db) => _db = db;

    public async Task<string> GenerateAsync(string userId, OtpPurpose purpose)
    {
        var now = DateTime.UtcNow;

        var last = await _db.OtpCodes
            .Where(o => o.UserId == userId && o.Purpose == purpose)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (last != null && last.CreatedAt > now.AddSeconds(-ResendCooldownSeconds))
            throw new AppException("Please wait a minute before requesting a new code.", 429);

        // بطّل أي كود قديم لسه شغال
        var active = await _db.OtpCodes
            .Where(o => o.UserId == userId && o.Purpose == purpose && o.ConsumedAt == null)
            .ToListAsync();
        foreach (var o in active) o.ConsumedAt = now;

        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

        _db.OtpCodes.Add(new OtpCode
        {
            UserId = userId,
            Purpose = purpose,
            CodeHash = Hash(userId, purpose, code),
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(ExpiryMinutes)
        });
        await _db.SaveChangesAsync();

        return code;
    }

    public async Task<bool> VerifyAsync(string userId, OtpPurpose purpose, string code)
    {
        var now = DateTime.UtcNow;

        var otp = await _db.OtpCodes
            .Where(o => o.UserId == userId && o.Purpose == purpose && o.ConsumedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp == null || otp.ExpiresAt <= now || otp.Attempts >= MaxAttempts)
            return false;

        otp.Attempts++;

        var expected = Encoding.UTF8.GetBytes(otp.CodeHash);
        var actual = Encoding.UTF8.GetBytes(Hash(userId, purpose, code));
        var ok = CryptographicOperations.FixedTimeEquals(expected, actual);

        if (ok) otp.ConsumedAt = now;

        await _db.SaveChangesAsync();
        return ok;
    }

    private static string Hash(string userId, OtpPurpose purpose, string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{userId}:{purpose}:{code}")));
}
