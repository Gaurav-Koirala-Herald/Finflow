namespace RoleBaseAuthorization.DTO
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSystemRole { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<int> FunctionIds { get; set; } = new();
        public List<int> PrivilegeIds { get; set; } = new();
    }

    public class CreateRoleDto
    {
        public string Name { get; set; } = string. Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AssignFunctionsDto
    {
        public int RoleId { get; set; }
        public List<int> FunctionIds { get; set; } = new();
    }

    public class AssignPrivilegesDto
    {
        public int RoleId { get; set; }
        public List<int> PrivilegeIds { get; set; } = new();
    }
}