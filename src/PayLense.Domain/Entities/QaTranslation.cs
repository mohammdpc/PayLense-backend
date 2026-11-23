namespace PayLense.Domain.Entities;

public class QaTranslation
{
    public int QaId { get; set; }
    public string Language { get; set; } = "en";
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;

    public QaItem? QaItem { get; set; }
}
