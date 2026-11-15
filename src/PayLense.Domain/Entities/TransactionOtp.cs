namespace PayLense.Domain.Entities;

public class TransactionOtp
{
    public long OtpId { get; set; }
    public Guid TransactionId { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int AttemptsCount { get; set; } = 0;
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Transaction? Transaction { get; set; }
}
