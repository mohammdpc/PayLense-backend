namespace PayLense.Domain.Entities;

public class MoneyRequest
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public string RequesterId { get; set; } = string.Empty;
    public string PayerId { get; set; } = string.Empty;
    public Guid? RequesterCardId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string? Note { get; set; }
    public string Status { get; set; } = "Pending"; // Pending/Accepted/Rejected/Cancelled/Expired
    public Guid? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RespondedAt { get; set; }

    public BankCard? RequesterCard { get; set; }
    public Transaction? Transaction { get; set; }
}
