using FinFlowAPI.DTO;

namespace FinFlowAPI.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto? > LoginAsync(LoginRequestDto request);
        Task<CommonResponseDTO> RegisterAsync(RegisterRequestDto request);
        Task<bool> ValidateTokenAsync(string token);
        CommonResponseDTO VerifyOtp(string email, string otp);
        string GenerateJwtToken(int userId, string username,bool IsActive);
    }
}