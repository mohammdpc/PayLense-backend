namespace PayLense.Domain.Entities;

public class AuthenticationChallenge
{
    public long ChallengeId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ChallengeType { get; set; } = "Login2FA";
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int AttemptsCount { get; set; } = 0;
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
