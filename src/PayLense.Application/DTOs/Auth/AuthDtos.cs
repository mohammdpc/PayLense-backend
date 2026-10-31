using System.ComponentModel.DataAnnotations;
using PayLense.Domain.Enums;

namespace PayLense.Application.DTOs.Auth;

// ---------- Requests ----------
public class SignupRequest
{
    [Required, RegularExpression(@"^[A-Za-z0-9._-]{3,30}$",
        ErrorMessage = "Username must be 3-30 chars: letters, digits, '.', '_' or '-'.")]
    public string Username { get; set; } = string.Empty;

    [Required, RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Invalid phone number.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string CardNumber { get; set; } = string.Empty;

    [Range(1, 12)]
    public int ExpiryMonth { get; set; }

    [Range(2000, 2100)]
    public int ExpiryYear { get; set; }
}

public class VerifyEmailRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, StringLength(6, MinimumLength = 6)] public string Otp { get; set; } = string.Empty;
}

public class VerifyPhoneRequest
{
    [Required] public string PhoneNumber { get; set; } = string.Empty;
    [Required, StringLength(6, MinimumLength = 6)] public string Otp { get; set; } = string.Empty;
}

public class ResendOtpRequest
{
    public OtpPurpose Purpose { get; set; }

    // EmailConfirmation => email | PhoneConfirmation => phone | LoginTwoFactor => email أو username
    [Required] public string Identifier { get; set; } = string.Empty;
}

public class SigninRequest
{
    // email أو username
    [Required] public string Identifier { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public string DeviceId { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
}

public class VerifyTwoFactorRequest
{
    [Required] public string Identifier { get; set; } = string.Empty;
    [Required, StringLength(6, MinimumLength = 6)] public string Otp { get; set; } = string.Empty;
    [Required] public string DeviceId { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public bool TrustDevice { get; set; }
}

public class RefreshRequest
{
    [Required] public string RefreshToken { get; set; } = string.Empty;
    [Required] public string DeviceId { get; set; } = string.Empty;
}

public class LogoutRequest
{
    [Required] public string RefreshToken { get; set; } = string.Empty;
}

// ---------- Responses ----------
public class MessageResponse
{
    public string Message { get; set; } = string.Empty;
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = new();
}

public class SigninResponse
{
    public bool RequiresTwoFactor { get; set; }
    public string? Message { get; set; }
    public AuthResponse? Auth { get; set; }
}
