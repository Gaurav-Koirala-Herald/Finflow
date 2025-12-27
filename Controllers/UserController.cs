using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinflowAPI.Models;
using FinflowAPI.Models.RequestModels. UserManagement;
using FinflowAPI.Services.UserManagement;
using FinflowAPI.Services.UserManagement;

namespace FinflowAPI. Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // [HttpGet("profile")]
    // public async Task<IActionResult> GetProfile()
    // {
    //     var userId = GetCurrentUserId();
    //     if (userId == null)
    //     {
    //         return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
    //     }
    //
    //     var result = await _userService.GetUserByIdAsync(userId. Value);
    //
    //     if (! result.Success)
    //     {
    //         return NotFound(result);
    //     }
    //
    //     return Ok(result);
    // }

    // [HttpPut("profile")]
    // public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request)
    // {
    //     var userId = GetCurrentUserId();
    //     if (userId == null)
    //     {
    //         return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
    //     }
    //
    //     var result = await _userService.UpdateUserAsync(userId.Value, request);
    //
    //     if (!result. Success)
    //     {
    //         return BadRequest(result);
    //     }
    //
    //     return Ok(result);
    // }

    // [HttpPost("change-password")]
    // public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(ApiResponse<object>.FailureResponse(
    //             "Validation failed",
    //             ModelState.Values.SelectMany(v => v.Errors.Select(e => e. ErrorMessage)).ToList()));
    //     }
    //
    //     var userId = GetCurrentUserId();
    //     if (userId == null)
    //     {
    //         return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user"));
    //     }
    //
    //     var result = await _userService.ChangePasswordAsync(userId.Value, request);
    //
    //     if (!result.Success)
    //     {
    //         return BadRequest(result);
    //     }
    //
    //     return Ok(result);
    // }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User. FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int. TryParse(userIdClaim, out int userId) ? userId : null;
    }
}