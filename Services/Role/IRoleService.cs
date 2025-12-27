using FinFlowAPI.DTO;

namespace FinFlowAPI.Services.Role
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<RoleDto?> CreateRoleAsync(CreateRoleDto dto, int createdById);
        Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto dto);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> AssignFunctionsAsync(AssignFunctionsDto dto);
        Task<bool> AssignPrivilegesAsync(AssignPrivilegesDto dto);
        Task<List<ModuleDto>> GetModulesWithFunctionsAndPrivilegesAsync();
        
        Task<List<FunctionDto>> GetFunctionsByRoleAsync(int roleId);
    }
}