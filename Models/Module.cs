using System.ComponentModel.DataAnnotations;

namespace RoleBaseAuthorization.Models
{
    public class Module
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string. Empty;
        
        [MaxLength(200)]
        public string Description { get; set; } = string. Empty;
        
        [MaxLength(100)]
        public string Icon { get; set; } = string.Empty;
        
        public int? ParentModuleId { get; set; }
        
        public int DisplayOrder { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual Module? ParentModule { get; set; }
        public virtual ICollection<Module> SubModules { get; set; } = new List<Module>();
        public virtual ICollection<Function> Functions { get; set; } = new List<Function>();
        public virtual ICollection<Privilege> Privileges { get; set; } = new List<Privilege>();
    }
}