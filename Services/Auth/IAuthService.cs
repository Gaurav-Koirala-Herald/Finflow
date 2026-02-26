using FinFlowAPI.DTO;

namespace FinFlowAPI.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto? > LoginAsync(LoginRequestDto request);
        Task<CommonResponseDTO> RegisterAsync(RegisterRequestDto request);
        Task<bool> ValidateTokenAsync(string token);
        LoginResponseDto VerifyOtp(string email, string otp);
        CommonResponseDTO ResendOtp(string email);
        string GenerateJwtToken(int userId, string username,bool IsActive);
    }
}