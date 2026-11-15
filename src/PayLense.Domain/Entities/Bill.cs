namespace PayLense.Domain.Entities;

public class Bill
{
    public Guid BillId { get; set; } = Guid.NewGuid();
    public string CreatorId { get; set; } = string.Empty;
    public string? ReceiverId { get; set; }
    public int? CategoryId { get; set; }
    public string BillName { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string PublicTokenHash { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Category? Category { get; set; }
    public ICollection<BillParticipant> Participants { get; set; } = new List<BillParticipant>();
    public ICollection<BillSettlement> Settlements { get; set; } = new List<BillSettlement>();
}
