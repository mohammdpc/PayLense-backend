namespace PayLense.Domain.Entities;

public class ReportSnapshot
{
    public Guid ReportId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public string Currency { get; set; } = "EGP";
    public string ReportData { get; set; } = "{}";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
