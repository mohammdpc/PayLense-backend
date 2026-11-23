namespace PayLense.Domain.Entities;

public class Receipt
{
    public Guid ReceiptId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public Guid? TransactionId { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string? MerchantName { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public DateOnly? ReceiptDate { get; set; }
    public int? CategoryId { get; set; }
    public int? SuggestedCategoryId { get; set; }
    public decimal? AiConfidence { get; set; }
    public bool IncludeInReport { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Transaction? Transaction { get; set; }
    public Category? Category { get; set; }
    public Category? SuggestedCategory { get; set; }
}
