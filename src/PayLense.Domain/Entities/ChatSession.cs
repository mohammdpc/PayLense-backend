namespace PayLense.Domain.Entities;

public class ChatSession
{
    public Guid ChatSessionId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string SessionStatus { get; set; } = "Active";
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
