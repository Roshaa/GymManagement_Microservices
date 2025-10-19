using AutoMapper;
using GymManagement_Auth_Microservice.Context;
using GymManagement_Auth_Microservice.DTO_s;
using GymManagement_Auth_Microservice.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Auth_Microservice.Services
{
    public class AuthService (
        ApplicationDbContext _context,
        UserManager<IdentityUser> _userManager,
        RoleManager<IdentityRole> roleManager,
        JwtTokenGenerator _jwtTokenGenerator,
         IMapper _mapper) 
    {

        public async Task<LoginResponseDTO> LoginAsync(LoginUserDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null) return null;

            bool valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) return null;

            UserDTO userDTO = _mapper.Map<UserDTO>(user);

            string jwtToken = await _jwtTokenGenerator.GenerateTokenAsync(user);

            return new LoginResponseDTO
            {
                User = userDTO,
                Token = jwtToken
            };
        }

        public async Task<IdentityResult> RegisterUserAsync (RegisterUserDTO dto)
        {
            var newUser = new IdentityUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
            return await _userManager.CreateAsync(newUser, dto.Password);
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return _mapper.Map<UserDTO>(user);
        }

    }
}
