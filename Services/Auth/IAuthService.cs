using RoleBaseAuthorization.DTO;

namespace RoleBaseAuthorization.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto? > LoginAsync(LoginRequestDto request);
        Task<UserDto?> RegisterAsync(RegisterRequestDto request, int createdById);
        Task<bool> ValidateTokenAsync(string token);
        string GenerateJwtToken(int userId, string username,bool IsActive, List<string> roles, List<string> functions, List<string> privileges);
    }
}