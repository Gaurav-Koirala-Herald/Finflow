using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinFlowAPI;
using FinFlowAPI.Attributes;
using FinFlowAPI.DTO;
using FinFlowAPI.Models;
using FinFlowAPI.Services.Role;

namespace FinFlowAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [RequirePrivilege("VIEW_ROLES")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        [RequirePrivilege("VIEW_ROLES")]
        public async Task<IActionResult> GetRole(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            var userId = int.Parse(User. FindFirst(ClaimTypes.NameIdentifier)?.Value ??  "0");
            var role = await _roleService.CreateRoleAsync(dto, userId);
            
            if (role == null)
                return BadRequest(new { message = "Role with this name already exists" });

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto dto)
        {
            var role = await _roleService. UpdateRoleAsync(id, dto);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (! result) return BadRequest(new { message = "Cannot delete this role" });
            return NoContent();
        }

        [HttpPost("{id}/functions")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignFunctions(int id, [FromBody] List<int> functionIds)
        { 
            var result = await _roleService.AssignFunctionsAsync(new AssignFunctionsDto
            {
                RoleId = id,
                FunctionIds = functionIds
            });
            return Ok(new { message = "Functions assigned successfully" });
        }

        [HttpPost("{id}/privileges")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AssignPrivileges(int id, [FromBody] List<int> privilegeIds)
        {
            var result = await _roleService.AssignPrivilegesAsync(new AssignPrivilegesDto
            {
                RoleId = id,
                PrivilegeIds = privilegeIds
            });

            if (!result) return NotFound();
            return Ok(new { message = "Privileges assigned successfully" });
        }

        [HttpGet("modules")]
        public async Task<IActionResult> GetModules()
        {
            var modules = await _roleService.GetModulesWithFunctionsAndPrivilegesAsync();
            return Ok(modules);
        }

        [HttpGet("{id}/role-functions")]
        public async Task<IActionResult> GetFunctionsByRoles(int roleId)
        {
            var userRoles = await _roleService.GetFunctionsByRoleAsync(roleId);
            return Ok(userRoles);
        }

      
    }
}