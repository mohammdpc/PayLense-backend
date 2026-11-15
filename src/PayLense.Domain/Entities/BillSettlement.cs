namespace PayLense.Domain.Entities;

public class BillSettlement
{
    public Guid SettlementId { get; set; } = Guid.NewGuid();
    public Guid BillId { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Bill? Bill { get; set; }
}
