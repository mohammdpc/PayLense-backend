namespace PayLense.Domain.Entities;

public class EmailChangeRequest
{
    public long RequestId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long OldContactId { get; set; }
    public string NewEmail { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending/Completed/Rejected/Expired
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public UserContactMethod? OldContact { get; set; }
}
