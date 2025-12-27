namespace FinflowAPI. Models.Entities;

public class UserSettings
{
    public int SettingId { get; set; }
    public int UserId { get; set; }
    public string Currency { get; set; } = "NPR";
    public string DateFormat { get; set; } = "YYYY-MM-DD";
    public string TimeFormat { get; set; } = "24h";
    public string FontSize { get; set; } = "Medium";
    public bool HighContrast { get; set; } = false;
    public bool ScreenReader { get; set; } = false;
    public bool NotificationsEnabled { get; set; } = true;
    public bool EmailNotifications { get; set; } = true;
    public bool SMSNotifications { get; set; } = false;

    // Navigation Property
    public virtual User User { get; set; } = null!;
}