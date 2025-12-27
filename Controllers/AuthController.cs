using Microsoft.AspNetCore.Authorization;
using Microsoft. AspNetCore. Mvc;
using System.Security.Claims;
using FinFlowAPI.DTO;
using FinFlowAPI.Services.Auth;

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

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var userId = int.Parse(User. FindFirst(ClaimTypes.NameIdentifier)?.Value ??  "0");
            var result = await _authService.RegisterAsync(request, userId);
            
            if (result == null)
                return BadRequest(new { message = "Username or email already exists" });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes. Name)?.Value;
            var roles = User.FindAll(ClaimTypes. Role). Select(c => c.Value). ToList();
            var functions = User.FindAll("function").Select(c => c.Value). ToList();
            var privileges = User. FindAll("privilege"). Select(c => c.Value).ToList();
            var status = User. FindAll("Status"). Select(c => c.Value).ToList();
            
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