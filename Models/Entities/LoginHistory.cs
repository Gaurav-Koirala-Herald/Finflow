namespace FinflowAPI.Models.Entities;

public class LoginHistory
{
    public int LoginId { get; set; }
    public int UserId { get; set; }
    public string?  IPAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public virtual User User { get; set; } = null!;
}