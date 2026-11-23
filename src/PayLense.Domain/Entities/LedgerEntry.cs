namespace PayLense.Domain.Entities;

public class LedgerEntry
{
    public long EntryId { get; set; }
    public Guid TransactionId { get; set; }
    public Guid AccountId { get; set; }
    public string EntryType { get; set; } = "Debit"; // Debit/Credit
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Transaction? Transaction { get; set; }
    public FinancialAccount? Account { get; set; }
}
