namespace PayLense.Domain.Entities;

public class ContactVerificationChallenge
{
    public long VerificationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long ContactId { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int AttemptsCount { get; set; } = 0;
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserContactMethod? Contact { get; set; }
}
