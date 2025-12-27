namespace RoleBaseAuthorization.DTO
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int?  ParentModuleId { get; set; }
        public int DisplayOrder { get; set; }
        public List<FunctionDto> Functions { get; set; } = new();
        public List<PrivilegeDto> Privileges { get; set; } = new();
        public List<ModuleDto> SubModules { get; set; } = new();
    }

    public class FunctionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string. Empty;
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
    }

    public class PrivilegeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string. Empty;
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
    }
}