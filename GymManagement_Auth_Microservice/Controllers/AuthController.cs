using GymManagement_Auth_Microservice.DTO_s;
using GymManagement_Auth_Microservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement_Auth_Microservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Only admins can register user, admins are seeded by default
    public class AuthController(AuthService _authService) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO dto)
        {
            var loginResponse = await _authService.LoginAsync(dto);
            if (loginResponse == null) return Unauthorized("Invalid Login");

            return Ok(loginResponse);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDTO dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var result = await _authService.RegisterUserAsync(dto);

            if (!result.Succeeded) return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            UserDTO user = await _authService.GetUserByEmail(dto.Email);

            return Ok(user);
        }

    }
}