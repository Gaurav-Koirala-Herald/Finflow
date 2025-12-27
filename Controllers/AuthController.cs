using System.Security.Claims;
using Microsoft.AspNetCore. Authorization;
using Microsoft. AspNetCore. Mvc;
using FinflowAPI. Models;
using FinflowAPI.Models.RequestModels;
using FinflowAPI.Models.RequestModels.UserManagement;
using FinflowAPI.RequestModels;
using FinflowAPI.Services.Auth;
using FinflowAPI. Services.Auth;

namespace FinflowAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>. FailureResponse(
                "Validation failed",
                ModelState. Values.SelectMany(v => v.Errors. Select(e => e.ErrorMessage)).ToList()));
        }

        var result = await _authService.RegisterAsync(request);

        if (! result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>. FailureResponse(
                "Validation failed",
                ModelState.Values.SelectMany(v => v. Errors.Select(e => e.ErrorMessage)).ToList()));
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?. ToString();
        var deviceInfo = Request.Headers["User-Agent"].ToString();

        var result = await _authService.LoginAsync(request, ipAddress, deviceInfo);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    // [HttpPost("refresh-token")]
    // public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    // {
    //     if (!ModelState. IsValid)
    //     {
    //         return BadRequest(ApiResponse<object>.FailureResponse("Invalid request"));
    //     }
    //
    //     var result = await _authService.RefreshTokenAsync(request);
    //
    //     if (!result.Success)
    //     {
    //         return Unauthorized(result);
    //     }
    //
    //     return Ok(result);
    // }

    // [Authorize]
    // [HttpPost("logout")]
    // public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    // {
    //     var userId = GetCurrentUserId();
    //     if (userId == null)
    //     {
    //         return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
    //     }
    //
    //     var result = await _authService.LogoutAsync(userId.Value, request.RefreshToken);
    //     return Ok(result);
    // }

    // [Authorize]
    // [HttpPost("revoke-all-tokens")]
    // public async Task<IActionResult> RevokeAllTokens()
    // {
    //     var userId = GetCurrentUserId();
    //     if (userId == null)
    //     {
    //         return Unauthorized(ApiResponse<object>. FailureResponse("Invalid user"));
    //     }
    //
    //     var result = await _authService.RevokeAllTokensAsync(userId.Value);
    //     return Ok(result);
    // }

    private int?  GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : null;
    }
}

public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}