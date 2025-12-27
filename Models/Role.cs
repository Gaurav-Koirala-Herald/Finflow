using System.ComponentModel.DataAnnotations;

namespace RoleBaseAuthorization.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Description { get; set; } = string. Empty;
        
        public bool IsSystemRole { get; set; } = false; // For SuperAdmin, cannot be deleted
        
        public int?  CreatedById { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime?  UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual User?  CreatedBy { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<RoleFunction> RoleFunctions { get; set; } = new List<RoleFunction>();
        public virtual ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();
    }
}