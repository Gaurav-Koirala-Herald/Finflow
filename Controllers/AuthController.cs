using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinFlowAPI.DTO;
using FinFlowAPI.Services.Auth;
using System.Net;
using FinFlowAPI.Services;

namespace FinFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);

            if (result.code == HttpStatusCode.BadRequest)
                return BadRequest(new { result.message });

            return Ok(result);
        }
        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            var result = _authService.VerifyOtp(request.Email, request.Otp);
            if (result.code == HttpStatusCode.OK)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public IActionResult ResendOtp(string email)
        {
            var result = _authService.ResendOtp(email);

            if (result.code == HttpStatusCode.OK)
                return Ok(result);

            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var functions = User.FindAll("function").Select(c => c.Value).ToList();
            var privileges = User.FindAll("privilege").Select(c => c.Value).ToList();
            var status = User.FindAll("Status").Select(c => c.Value).ToList();

            return Ok(new
            {
                userId,
                username,
                roles,
                functions,
                privileges,
                status
            });
        }
    }
}