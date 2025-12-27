using Microsoft.AspNetCore. Authorization;
using Microsoft. AspNetCore. Mvc;
using FinflowAPI. Models;
using FinflowAPI. Models.RequestModels.UserManagement;
using FinflowAPI.Services.Auth;
using FinflowAPI.Services.UserManagement;

namespace FinflowAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    // [HttpGet("users")]
    // public async Task<IActionResult> GetAllUsers(
    //     [FromQuery] int pageNumber = 1,
    //     [FromQuery] int pageSize = 10,
    //     [FromQuery] string?  searchTerm = null,
    //     [FromQuery] bool? isActive = null)
    // {
    //     var result = await _userService.GetAllUsersAsync(pageNumber, pageSize, searchTerm, isActive);
    //
    //     if (!result.Success)
    //     {
    //         return BadRequest(result);
    //     }
    //
    //     return Ok(result);
    // }

    // [HttpGet("users/{userId}")]
    // public async Task<IActionResult> GetUserById(int userId)
    // {
    //     var result = await _userService.GetUserByIdAsync(userId);
    //
    //     if (!result.Success)
    //     {
    //         return NotFound(result);
    //     }
    //
    //     return Ok(result);
    // }
    //
    // [HttpPut("users/{userId}")]
    // public async Task<IActionResult> UpdateUser(int userId, [FromBody] AdminUpdateUserRequest request)
    // {
    //     var result = await _userService.AdminUpdateUserAsync(userId, request);
    //
    //     if (!result.Success)
    //     {
    //         return BadRequest(result);
    //     }
    //
    //     return Ok(result);
    // }

    [HttpPost("users/{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        var result = await _userService.DeactivateUserAsync(userId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("users/{userId}/activate")]
    public async Task<IActionResult> ActivateUser(int userId)
    {
        var result = await _userService. ActivateUserAsync(userId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var result = await _userService. DeleteUserAsync(userId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("users/{userId}/roles/{roleId}")]
    public async Task<IActionResult> AssignRole(int userId, int roleId)
    {
        var result = await _userService.AssignRoleAsync(userId, roleId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("users/{userId}/roles/{roleId}")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var result = await _userService.RemoveRoleAsync(userId, roleId);

        if (!result. Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _userService.GetAllRolesAsync();

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}