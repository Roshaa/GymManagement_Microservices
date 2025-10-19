using GymManagement_Auth_Microservice.Context;
using GymManagement_Auth_Microservice.DTO_s;
using GymManagement_Auth_Microservice.DTO_s.GymManagement_Auth_Microservice.DTO_s;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Auth_Microservice.Services
{
    public class UserService(ApplicationDbContext _context, UserManager<IdentityUser> _userManager)
    {

        public async Task<UserDTO[]> GetUsersAsync(int page = 1)
        {
            int usersPerPage = 20;
            int skipRows = (page - 1) * usersPerPage;

            return  await _context.Users.AsNoTracking().OrderBy(u => u.UserName)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Phone = u.PhoneNumber
                })
                .Skip(skipRows)
                .Take(usersPerPage)
                .ToArrayAsync();
        }

        public async Task<UserDetailsDTO> GetUserDetailsAsync(string Id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == Id)
                .Select(u => new UserDetailsDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = u.EmailConfirmed,
                    PhoneNumber = u.PhoneNumber,
                    PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                    TwoFactorEnabled = u.TwoFactorEnabled,
                    LockoutEnabled = u.LockoutEnabled,
                    LockoutEnd = u.LockoutEnd,
                    AccessFailedCount = u.AccessFailedCount
                })
                .SingleOrDefaultAsync();
        }

        public async Task<(bool NotFound, List<string> Errors)> UpdateUserAsync(string id, UserUpdateDTO dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return (true, new List<string>());

            var errors = new List<string>();

            if (!string.Equals(user.UserName, dto.Name, StringComparison.Ordinal))
            {
                var setNameResult = await _userManager.SetUserNameAsync(user, dto.Name);
                if (!setNameResult.Succeeded)
                    errors.AddRange(setNameResult.Errors.Select(e => e.Description));
            }

            if (!string.Equals(user.PhoneNumber, dto.Phone, StringComparison.Ordinal))
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, dto.Phone);
                if (!setPhoneResult.Succeeded)
                    errors.AddRange(setPhoneResult.Errors.Select(e => e.Description));
            }

            return (false, errors);
        }

        public async Task<(bool NotFound, List<string> Errors)> DeleteUserAsync(string id, string? currentUserId)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return (true, new List<string>());

            if (!string.IsNullOrEmpty(currentUserId) && currentUserId == id)
                return (false, new List<string> { "You cannot delete your own account." });

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1) return (false, new List<string> { "Cannot delete the last remaining Admin." });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) return (false, result.Errors.Select(e => e.Description).ToList());

            return (false, new List<string>());
        }


    }
}
