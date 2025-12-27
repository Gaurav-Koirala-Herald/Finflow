namespace RoleBaseAuthorization.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string. Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<RoleDto> Roles { get; set; } = new();
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = string. Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<int> RoleIds { get; set; } = new();
    }

    public class UpdateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class AssignRolesDto
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}