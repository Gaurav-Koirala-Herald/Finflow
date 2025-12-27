using System.ComponentModel.DataAnnotations;

namespace FinFlowAPI.Models
{
    public class RoleFunction
    {
        [Key]
        public int Id { get; set; }
        
        public int RoleId { get; set; }
        
        public int FunctionId { get; set; }
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual Function Function { get; set; } = null!;
    }
}