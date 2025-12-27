namespace FinflowAPI. Models.Entities;

public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string. Empty;
    public string?  Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime. UtcNow;

    // Navigation Property
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}