namespace PayLense.Domain.Entities;

public class Transaction
{
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public string SenderId { get; set; } = string.Empty;
    public Guid? SenderCardId { get; set; }
    public string? ReceiverId { get; set; }
    public Guid? ReceiverCardId { get; set; }
    public string? ExternalCardToken { get; set; }
    public string? ExternalCardLast4 { get; set; }
    public int? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string SendType { get; set; } = "Instant"; // Instant/Delayed/Secure/Recurring
    public string Source { get; set; } = "Direct"; // Direct/Request/Bill/Automated
    public string Status { get; set; } = "Pending"; // Pending/Processing/Completed/Failed/Cancelled/Reversed
    public bool ExceedsLimit { get; set; } = false;
    public DateTime? LimitOverrideConfirmedAt { get; set; }
    public bool Requires2Fa { get; set; } = false;
    public DateTime? TwoFaVerifiedAt { get; set; }
    public Guid IdempotencyKey { get; set; } = Guid.NewGuid();
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public BankCard? SenderCard { get; set; }
    public BankCard? ReceiverCard { get; set; }
    public Category? Category { get; set; }
    public DelayedSend? DelayedSend { get; set; }
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
    public ICollection<TransactionStatusHistory> StatusHistory { get; set; } = new List<TransactionStatusHistory>();
    public ICollection<TransactionOtp> TransactionOtps { get; set; } = new List<TransactionOtp>();
    public ICollection<MoneyRequest> MoneyRequests { get; set; } = new List<MoneyRequest>();
    public ICollection<AutomatedSendRun> AutomatedSendRuns { get; set; } = new List<AutomatedSendRun>();
    public ICollection<BillParticipant> BillParticipants { get; set; } = new List<BillParticipant>();
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
