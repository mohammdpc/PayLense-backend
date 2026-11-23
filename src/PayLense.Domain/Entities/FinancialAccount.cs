namespace PayLense.Domain.Entities;

public class FinancialAccount
{
    public Guid AccountId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string Currency { get; set; } = "EGP";
    public string AccountStatus { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<BankCard> BankCards { get; set; } = new List<BankCard>();
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
    public ICollection<SpendingLimit> SpendingLimits { get; set; } = new List<SpendingLimit>();
}
