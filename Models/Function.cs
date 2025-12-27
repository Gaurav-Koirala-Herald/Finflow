using System.ComponentModel.DataAnnotations;

namespace FinFlowAPI.Models
{
    public class Function
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = string.Empty; // e.g., "PREVIEW_DASHBOARD", "FILTER_DATE"
        
        public int ModuleId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual Module Module { get; set; } = null!;
        public virtual ICollection<RoleFunction> RoleFunctions { get; set; } = new List<RoleFunction>();
    }
}