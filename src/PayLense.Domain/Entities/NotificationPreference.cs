namespace PayLense.Domain.Entities;

public class NotificationPreference
{
    public string UserId { get; set; } = string.Empty;
    public bool TransactionNotifications { get; set; } = true;
    public bool SecurityNotifications { get; set; } = true;
    public bool MarketingNotifications { get; set; } = false;
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
