namespace PayLense.Domain.Entities;

public class DelayedSend
{
    public Guid TransactionId { get; set; }
    public string? MessageText { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? ReceiverNotifiedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public Transaction? Transaction { get; set; }
}
