using RoleBaseAuthorization.DTO;

namespace RoleBaseAuthorization.Services.User
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> CreateUserAsync(CreateUserDto dto, int createdById);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> AssignRolesAsync(AssignRolesDto dto, int assignedById);
        
        Task<List<FunctionDto>> GetFunctionsByUserIdAsync(int Id);

        Task<bool> UpdateUserProfileAsync(UserDto userDto);
    }
}