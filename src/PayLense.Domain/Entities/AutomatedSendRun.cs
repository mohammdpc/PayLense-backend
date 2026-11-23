namespace PayLense.Domain.Entities;

public class AutomatedSendRun
{
    public long RunId { get; set; }
    public Guid AutomatedSendId { get; set; }
    public DateTime ScheduledFor { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public Guid? TransactionId { get; set; }
    public string? FailureReason { get; set; }

    public AutomatedSend? AutomatedSend { get; set; }
    public Transaction? Transaction { get; set; }
}
