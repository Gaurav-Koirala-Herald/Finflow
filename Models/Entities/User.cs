namespace FinflowAPI.Models.Entities;

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string?  MobileNumber { get; set; }
    public string PasswordHash { get; set; } = string. Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string?  Gender { get; set; }
    public string?  ProfilePicture { get; set; }
    public string PreferredLanguage { get; set; } = "en";
    public bool IsEmailVerified { get; set; } = false;
    public bool IsMobileVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime?  UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties
    public virtual UserSettings?  UserSettings { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}