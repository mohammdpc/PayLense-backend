namespace PayLense.Domain.Entities;

public class LoginSession
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string SessionTokenHash { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime? TwoFaVerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
