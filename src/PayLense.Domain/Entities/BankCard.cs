namespace PayLense.Domain.Entities;

public class BankCard
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public Guid? AccountId { get; set; }

    // مشفّر بـ AES-GCM - عمره ما يتخزن plain text
    public string CardNumberEncrypted { get; set; } = string.Empty;
    public string? CardToken { get; set; }
    public string CardFingerprint { get; set; } = string.Empty;
    public string Last4Digits { get; set; } = string.Empty;
    public string? CardholderName { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = "EGP";
    public string CardStatus { get; set; } = "Active";
    public bool IsDefault { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public FinancialAccount? Account { get; set; }
    public ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
    public ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
    public ICollection<MoneyRequest> MoneyRequests { get; set; } = new List<MoneyRequest>();
    public ICollection<AutomatedSend> SourceAutomatedSends { get; set; } = new List<AutomatedSend>();
    public ICollection<AutomatedSend> ReceiverAutomatedSends { get; set; } = new List<AutomatedSend>();
    public ICollection<BillParticipant> BillParticipants { get; set; } = new List<BillParticipant>();
}
