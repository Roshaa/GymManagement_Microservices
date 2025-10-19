using GymManagement_Auth_Microservice.DTO_s;
using GymManagement_Auth_Microservice.DTO_s.GymManagement_Auth_Microservice.DTO_s;
using GymManagement_Auth_Microservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement_Auth_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController( UserService _userService, UserManager<IdentityUser> _userManager) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetUsers(int page = 1)
        {
            UserDTO[] users = await _userService.GetUsersAsync(page);

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) return BadRequest();

            UserDetailsDTO user = await _userService.GetUserDetailsAsync(id);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var result = await _userService.UpdateUserAsync(id, dto);
            if (result.NotFound) return NotFound();
            if (result.Errors.Count > 0) return BadRequest(new { errors = result.Errors });

            var updated = await _userService.GetUserDetailsAsync(id);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            string currentUserId = _userManager.GetUserId(User);

            var result = await _userService.DeleteUserAsync(id, currentUserId);

            if (result.NotFound) return NotFound();
            if (result.Errors.Count > 0) return BadRequest(new { errors = result.Errors });

            return NoContent();
        }

    }
}
