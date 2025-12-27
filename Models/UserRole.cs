using System.ComponentModel. DataAnnotations;

namespace RoleBaseAuthorization.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        public int RoleId { get; set; }
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
        public int? AssignedById { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
        public virtual User? AssignedBy { get; set; }
    }
}