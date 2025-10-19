using GymManagement_Auth_Microservice.DTO_s;
using GymManagement_Auth_Microservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement_Auth_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController(RoleService _roleService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult>GetRoles()
        {
            RoleDTO[] roles = await _roleService.GetRolesAsync();
            return Ok(roles);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> SetUserRoles(string userId, [FromBody] string[] roleIds)
        {
            if (roleIds is null || roleIds.Length == 0) return BadRequest("Specify the roles");

            var (dto, invalidRoleIds, errors) = await _roleService.SetUserRolesAsync(userId, roleIds);

            if (errors.Length > 0) return BadRequest(new { errors, invalidRoleIds });
            if (invalidRoleIds.Length > 0) Response.Headers.Append("Invalid-RoleIds", string.Join(",", invalidRoleIds));

            return Ok(dto);
        }

    }
}
