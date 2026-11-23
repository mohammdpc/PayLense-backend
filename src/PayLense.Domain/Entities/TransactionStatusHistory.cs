namespace PayLense.Domain.Entities;

public class TransactionStatusHistory
{
    public long HistoryId { get; set; }
    public Guid TransactionId { get; set; }
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string? ChangedBy { get; set; }
    public string? ChangeReason { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public Transaction? Transaction { get; set; }
}
