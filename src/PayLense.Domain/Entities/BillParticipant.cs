namespace PayLense.Domain.Entities;

public class BillParticipant
{
    public Guid BillId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid? CardId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public Guid? TransactionId { get; set; }

    public Bill? Bill { get; set; }
    public BankCard? Card { get; set; }
    public Transaction? Transaction { get; set; }
}
