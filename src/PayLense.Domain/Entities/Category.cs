namespace PayLense.Domain.Entities;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<AutomatedSend> AutomatedSends { get; set; } = new List<AutomatedSend>();
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    public ICollection<Receipt> SuggestedReceipts { get; set; } = new List<Receipt>();
}
