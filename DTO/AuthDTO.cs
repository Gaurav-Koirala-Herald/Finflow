using System.ComponentModel.DataAnnotations;

namespace FinFlowAPI.DTO
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<string> Functions { get; set; } = new();
        public List<string> Privileges { get; set; } = new();
    }

    public class RegisterRequestDto
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string. Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string. Empty;
        
        [Required]
        public string FullName { get; set; } = string. Empty;
        
        public List<int> RoleIds { get; set; } = new();
    }
}