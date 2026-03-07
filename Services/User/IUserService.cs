using FinFlowAPI.DTO;

namespace FinFlowAPI.Services.User
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

        Task<CommonResponseDTO> UpdatePreferencesAsync(
            UserPreferenceUpdateDTO dto, string userId);
        Task<UserDto?> GetUserProfileAsync(int id);
        Task<bool> UpdateUserProfileAsync(UserDto userDto);
    }
}