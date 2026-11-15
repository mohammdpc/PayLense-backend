namespace PayLense.Domain.Entities;

public class AuditLog
{
    public long LogId { get; set; }
    public string? UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
    public string? IpAddress { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
