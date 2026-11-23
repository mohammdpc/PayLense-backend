namespace PayLense.Domain.Entities;

public class AutomatedSend
{
    public Guid AutomatedSendId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public Guid? CardId { get; set; }
    public string? ReceiverId { get; set; }
    public Guid? ReceiverCardId { get; set; }
    public string? ExternalCardToken { get; set; }
    public string? ExternalCardLast4 { get; set; }
    public int? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string Frequency { get; set; } = "Monthly"; // Daily/Weekly/Monthly
    public int IntervalCount { get; set; } = 1;
    public byte? DayOfMonth { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime NextRunAt { get; set; }
    public DateTime? LastRunAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public BankCard? Card { get; set; }
    public BankCard? ReceiverCard { get; set; }
    public Category? Category { get; set; }
    public ICollection<AutomatedSendRun> Runs { get; set; } = new List<AutomatedSendRun>();
}
