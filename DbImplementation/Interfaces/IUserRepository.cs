using FinflowAPI. Models.Entities;
using FinflowAPI.Models.ResponseModels;
using FinflowAPI. Models.ResponseModels.UserManagement;

namespace FinflowAPI. DbImplementation. Interfaces;

public interface IUserRepository 
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdWithRolesAsync(int userId);
    Task<User?> GetByEmailWithRolesAsync(string email);
    Task<bool> EmailExistsAsync(string email, int?  excludeUserId = null);
    Task<bool> MobileExistsAsync(string mobileNumber, int? excludeUserId = null);
    // Task<PaginatedResponse<UserResponse>> GetAllUsersPagedAsync(int pageNumber, int pageSize, string? searchTerm, bool? isActive);
    Task CreateUserWithSettingsAsync(User user, int defaultRoleId);
    Task UpdateUserRolesAsync(int userId, List<int> roleIds);
    Task<List<string>> GetUserRolesAsync(int userId);
}