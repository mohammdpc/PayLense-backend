namespace PayLense.Domain.Entities;

public class SpendingLimit
{
    public long LimitId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid? AccountId { get; set; }
    public decimal LimitAmount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string PeriodType { get; set; } = "Monthly"; // Daily/Weekly/Monthly
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public FinancialAccount? Account { get; set; }
}
