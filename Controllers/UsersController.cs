using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RoleBaseAuthorization.Attributes;
using RoleBaseAuthorization.DTO;
using RoleBaseAuthorization.Services.User;
using UMS. API.Services;

namespace UMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [RequirePrivilege("VIEW_STAFF_LIST")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [RequirePrivilege("VIEW_STAFF_DETAILS")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService. GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        [RequireFunction("ADD_STAFF")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var userId = int.Parse(User. FindFirst(ClaimTypes.NameIdentifier)?.Value ??  "0");
            var user = await _userService.CreateUserAsync(dto, userId);
            
            if (user == null)
                return BadRequest(new { message = "Username or email already exists" });

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [RequireFunction("EDIT_STAFF")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [RequireFunction("DELETE_STAFF")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return BadRequest(new { message = "Cannot delete this user" });
            return NoContent();
        }

        [HttpPost("{id}/roles")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignRoles(int id, [FromBody] List<int> roleIds)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes. NameIdentifier)?. Value ?? "0");
            var result = await _userService. AssignRolesAsync(new AssignRolesDto
            {
                UserId = id,
                RoleIds = roleIds
            }, userId);

            if (!result) return NotFound();
            return Ok(new { message = "Roles assigned successfully" });
        }

        [HttpGet("{id}/functions")]
        public async Task<IActionResult> GetFunctionsByUserIdAsync(int userId)
        {
            var result = await _userService.GetFunctionsByUserIdAsync(userId);
            return Ok(result);
        }
        
        [HttpPut("update-user-profile")]
        public async Task<IActionResult> UpdateProfile(UserDto userDto)
        {
            var result = await _userService.UpdateUserProfileAsync(userDto);
            return Ok(result);
        }
        [HttpGet("get-user-details/{id}")]
        public async Task<IActionResult> GetUserDetails(int id) => Ok(await _userService.GetUserByIdAsync(id));
    }
}