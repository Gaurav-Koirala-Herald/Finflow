using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinFlowAPI.Data;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FinFlowAPI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly CommonService _commonService;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthService(EmailService emailService, CommonService commonService, ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _commonService = commonService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

            if (user == null || !request.Password.Contains(user.PasswordHash))
                return null;

            var emailSentResponse = await _emailService.SendOtpEmail(user.Email);

            if (emailSentResponse.code != HttpStatusCode.OK)
                return null;

            var token = GenerateJwtToken(user.Id, user.Username, user.IsActive);

            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
            };
        }

        public async Task<CommonResponseDTO> RegisterAsync(RegisterRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
                return new CommonResponseDTO { code = HttpStatusCode.BadRequest, message = "Username or email already exists" };

            var user = new Models.User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password,
                FullName = request.FullName,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync();

            var emailSentResponse = await _emailService.SendOtpEmail(request.Email);
            if (emailSentResponse.code != HttpStatusCode.OK)
                return new CommonResponseDTO { code = HttpStatusCode.InternalServerError, message = "Failed to send OTP email" };
            if (result > 0)
                return new CommonResponseDTO { code = HttpStatusCode.OK, message = "User registered successfully" };
            else
                return new CommonResponseDTO { code = HttpStatusCode.InternalServerError, message = "Failed to register user" };

          }


        public LoginResponseDto VerifyOtp(string email, string otp)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.otp == otp && u.OtpCreatedDateTime.AddMinutes(5) > DateTime.Now);
            if (user != null)
            {
                user.IsActive = true;
                user.otp = null; 
                user.OtpCreatedDateTime = DateTime.Now;
                user.IsOtpVerified = true;
                _context.SaveChanges();
                var token = GenerateJwtToken(user.Id, user.Username, user.IsActive);

                return new LoginResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Token = token,
                    code = HttpStatusCode.OK,
                    message = "OTP verified successfully"
                };

            }
            return new LoginResponseDto { code = HttpStatusCode.BadRequest, message = "Invalid OTP or email" };
        }


        public CommonResponseDTO ResendOtp(string email)
        {
            var emailSend = _emailService.SendOtpEmail(email);
            if (emailSend.Result.code == HttpStatusCode.OK)
                return new CommonResponseDTO { code = HttpStatusCode.OK, message = "OTP resent successfully" };
            return new CommonResponseDTO { code = HttpStatusCode.BadRequest, message = "Email not found or already verified" };
        }

        public string GenerateJwtToken(int userId, string username, bool IsActive)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            claims.Add(new Claim("Status", IsActive.ToString()));


            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}