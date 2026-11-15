namespace PayLense.Domain.Entities;

public class AppSetting
{
    public string UserId { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "System";
    public bool NotificationsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
