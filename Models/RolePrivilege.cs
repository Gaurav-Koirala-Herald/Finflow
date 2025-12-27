using System. ComponentModel.DataAnnotations;

namespace FinFlowAPI.Models
{
    public class RolePrivilege
    {
        [Key]
        public int Id { get; set; }
        
        public int RoleId { get; set; }
        
        public int PrivilegeId { get; set; }
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual Privilege Privilege { get; set; } = null!;
    }
}