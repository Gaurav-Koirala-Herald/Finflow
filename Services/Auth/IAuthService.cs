using FinflowAPI.Models;
using FinflowAPI.Models.RequestModels;
using FinflowAPI.Models.RequestModels.UserManagement;
using FinflowAPI.Models.ResponseModels;
using FinflowAPI.Models.ResponseModels.UserManagement;
using FinflowAPI.RequestModels;

namespace FinflowAPI.Services.Auth;

public interface IAuthService
{
    Task<ApiResponse<RegisterUserResponse>> RegisterAsync(RegisterUserRequest request);
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, string?  ipAddress, string? deviceInfo);
    // Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    // Task<ApiResponse<bool>> LogoutAsync(int userId, string refreshToken);
    // Task<ApiResponse<bool>> RevokeAllTokensAsync(int userId);
}