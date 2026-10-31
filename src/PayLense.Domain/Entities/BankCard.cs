namespace PayLense.Domain.Entities;

public class BankCard
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;

    // مشفّر بـ AES-GCM - عمره ما يتخزن plain text
    public string CardNumberEncrypted { get; set; } = string.Empty;
    public string Last4Digits { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
