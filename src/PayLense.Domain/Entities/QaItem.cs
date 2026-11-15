namespace PayLense.Domain.Entities;

public class QaItem
{
    public int QaId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QaTranslation> Translations { get; set; } = new List<QaTranslation>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
