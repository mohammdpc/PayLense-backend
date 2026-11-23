namespace PayLense.Domain.Entities;

public class ChatMessage
{
    public long MessageId { get; set; }
    public Guid ChatSessionId { get; set; }
    public string SenderType { get; set; } = "User"; // User/Bot
    public string Content { get; set; } = string.Empty;
    public string? Language { get; set; }
    public int? MatchedQaId { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public bool IsFallback { get; set; } = false;
    public bool? IsHelpful { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ChatSession? ChatSession { get; set; }
    public QaItem? MatchedQa { get; set; }
}
