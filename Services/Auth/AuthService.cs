using System.Net;
using System.Security.Claims;
using FinflowAPI. DbImplementation. Interfaces;
using FinflowAPI.Models;
using FinflowAPI.Models.Entities;
using FinflowAPI.Models.RequestModels;
using FinflowAPI.Models. RequestModels.UserManagement;
using FinflowAPI.Models.ResponseModels;
using FinflowAPI. Models.ResponseModels.UserManagement;
using FinflowAPI.RequestModels;
using FinflowAPI.Services.Auth;
using FinflowAPI. Services.Auth;

namespace FinflowAPI.Services. Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        JwtTokenGenerator tokenGenerator,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<ApiResponse<RegisterUserResponse>> RegisterAsync(RegisterUserRequest request)
    {
        try
        {

            if (await _unitOfWork.Users.EmailExistsAsync(request.email))
            {
                return ApiResponse<RegisterUserResponse>. FailureResponse("Email already registered");
            }
            
            if (! string.IsNullOrEmpty(request.mobileNumber) &&
                await _unitOfWork. Users.MobileExistsAsync(request.mobileNumber))
            {
                return ApiResponse<RegisterUserResponse>.FailureResponse("Mobile number already registered");
            }
            
            // var userRole = await _unitOfWork.Roles.GetByNameAsync("User");
            // if (userRole == null)
            // {
            //     return ApiResponse<RegisterUserResponse>. FailureResponse("Default role not found");
            // }
            
            var user = new RegisterUserRequest()
            {
                email = request.email. ToLower().Trim(),
                passwordHash = BCrypt.Net.BCrypt.HashPassword(request.passwordHash),
                fullName = request.fullName. Trim(),
                mobileNumber = request.mobileNumber?. Trim(),
                dob = request.dob,
                gender = request.gender,
            };
            
            // await _unitOfWork.Users.CreateUserWithSettingsAsync(user, userRole.RoleId);

            var response = new RegisterUserResponse
            {
                message = "Registration successful.  Please verify your email.",
                responseCode = HttpStatusCode.OK
            };

            return ApiResponse<RegisterUserResponse>.SuccessResponse(response, "User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", request.email);
            return ApiResponse<RegisterUserResponse>. FailureResponse("An error occurred during registration");
        }
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, string? ipAddress, string? deviceInfo)
    {
        try
        {
            // Get user with roles
            var user = await _unitOfWork. Users.GetByEmailWithRolesAsync(request.email);

            // Validate credentials
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.PasswordHash))
            {
                // Log failed attempt if user exists
                if (user != null)
                {
                    // await _unitOfWork. LoginHistories.LogLoginAttemptAsync(user.UserId, ipAddress, deviceInfo, false);
                }
                return ApiResponse<LoginResponse>.FailureResponse("Invalid email or password");
            }

            // Check if user is active
            if (!user. IsActive)
            {
                // await _unitOfWork. LoginHistories.LogLoginAttemptAsync(user.UserId, ipAddress, deviceInfo, false);
                return ApiResponse<LoginResponse>.FailureResponse("Your account has been deactivated.  Please contact support.");
            }

            // Log successful login
            // await _unitOfWork. LoginHistories.LogLoginAttemptAsync(user.UserId, ipAddress, deviceInfo, true);

            // Get user roles
            var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

            // Generate tokens
            var accessToken = _tokenGenerator.GenerateAccessToken(request);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            // Save refresh token
            // await _unitOfWork.RefreshTokens.AddTokenAsync(
            //     user.UserId,
            //     refreshToken,
            //     _tokenGenerator.GetRefreshTokenExpiry());

            // Update last login
            user. LastLoginAt = DateTime. UtcNow;
            // _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var response = new LoginResponse
            {
                accessToken = accessToken,
                refreshToken = refreshToken,
                expiresAt = DateTime.Now.AddMinutes(30)
            };

            return ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful");
        }
        catch (Exception ex)
        {
            _logger. LogError(ex, "Error during login for email: {Email}", request.email);
            return ApiResponse<LoginResponse>.FailureResponse("An error occurred during login");
        }
    }

    // public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    // {
    //     try
    //     {
    //         // Validate expired access token
    //         var principal = _tokenGenerator. GetPrincipalFromExpiredToken(request.AccessToken);
    //         if (principal == null)
    //         {
    //             return ApiResponse<LoginResponse>. FailureResponse("Invalid access token");
    //         }
    //
    //         // Extract user ID from token
    //         var userIdClaim = principal.FindFirst(ClaimTypes. NameIdentifier)?.Value;
    //         if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
    //         {
    //             return ApiResponse<LoginResponse>. FailureResponse("Invalid token claims");
    //         }
    //
    //         // Validate refresh token
    //         var storedToken = await _unitOfWork.RefreshTokens.GetValidTokenAsync(userId, request.RefreshToken);
    //         if (storedToken == null)
    //         {
    //             return ApiResponse<LoginResponse>.FailureResponse("Invalid or expired refresh token");
    //         }
    //
    //         // Get user with roles
    //         var user = await _unitOfWork.Users.GetByIdWithRolesAsync(userId);
    //         if (user == null || !user.IsActive)
    //         {
    //             return ApiResponse<LoginResponse>.FailureResponse("User not found or inactive");
    //         }
    //
    //         // Revoke old token
    //         await _unitOfWork.RefreshTokens.RevokeTokenAsync(userId, request.RefreshToken);
    //
    //         // Get user roles
    //         var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
    //
    //         // Generate new tokens
    //         var newAccessToken = _tokenGenerator.GenerateAccessToken(user, roles);
    //         var newRefreshToken = _tokenGenerator.GenerateRefreshToken();
    //
    //         // Save new refresh token
    //         await _unitOfWork.RefreshTokens.AddTokenAsync(
    //             user.UserId,
    //             newRefreshToken,
    //             _tokenGenerator.GetRefreshTokenExpiry());
    //
    //         var response = new LoginResponse
    //         {
    //             UserId = user. UserId,
    //             Email = user.Email,
    //             FullName = user. FullName,
    //             AccessToken = newAccessToken,
    //             RefreshToken = newRefreshToken,
    //             AccessTokenExpiry = _tokenGenerator.GetAccessTokenExpiry(),
    //             Roles = roles
    //         };
    //
    //         return ApiResponse<LoginResponse>.SuccessResponse(response, "Token refreshed successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error during token refresh");
    //         return ApiResponse<LoginResponse>.FailureResponse("An error occurred during token refresh");
    //     }
    // }

    // public async Task<ApiResponse<bool>> LogoutAsync(int userId, string refreshToken)
    // {
    //     try
    //     {
    //         await _unitOfWork. RefreshTokens. RevokeTokenAsync(userId, refreshToken);
    //         return ApiResponse<bool>.SuccessResponse(true, "Logged out successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error during logout for user:  {UserId}", userId);
    //         return ApiResponse<bool>. FailureResponse("An error occurred during logout");
    //     }
    // }

    // public async Task<ApiResponse<bool>> RevokeAllTokensAsync(int userId)
    // {
    //     try
    //     {
    //         await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId);
    //         return ApiResponse<bool>. SuccessResponse(true, "All tokens revoked successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error revoking tokens for user: {UserId}", userId);
    //         return ApiResponse<bool>.FailureResponse("An error occurred while revoking tokens");
    //     }
    // }
}