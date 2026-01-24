using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace FinFlowAPI.Models;

public class User
{
    public int Id { get; set; }
        
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;
        
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
        
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
        
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
        
    public bool IsActive { get; set; } = true;
        
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
    public DateTime?  UpdatedAt { get; set; }
        
    public List<Goal> Goals { get; set; } = new();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}