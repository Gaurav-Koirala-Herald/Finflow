using FinflowAPI.Models;
using FinflowAPI.Models.Entities;
using FinflowAPI.Models.ResponseModels;

namespace FinflowAPI.Services.UserManagement;

public interface IUserService
{
    // Task<ApiResponse<UserResponse>> GetUserByIdAsync(int userId);
    // Task<ApiResponse<UserResponse>> UpdateUserAsync(int userId, UpdateUserRequest request);
    // Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordRequest request);

    // Admin User Management
    // Task<ApiResponse<PaginatedResponse<UserResponse>>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm, bool?  isActive);
    // Task<ApiResponse<UserResponse>> AdminUpdateUserAsync(int userId, AdminUpdateUserRequest request);
    Task<ApiResponse<bool>> DeactivateUserAsync(int userId);
    Task<ApiResponse<bool>> ActivateUserAsync(int userId);
    Task<ApiResponse<bool>> DeleteUserAsync(int userId);
    Task<ApiResponse<bool>> AssignRoleAsync(int userId, int roleId);
    Task<ApiResponse<bool>> RemoveRoleAsync(int userId, int roleId);
    Task<ApiResponse<List<Role>>> GetAllRolesAsync();
}