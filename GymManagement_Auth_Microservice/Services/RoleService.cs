using GymManagement_Auth_Microservice.Context;
using GymManagement_Auth_Microservice.DTO_s;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Auth_Microservice.Services
{
    public class RoleService(ApplicationDbContext _context, UserManager<IdentityUser> _userManager, RoleManager<IdentityRole> _roleManager)
    {

        public async Task<RoleDTO[]> GetRolesAsync()
        {
            return await _context.Roles.AsNoTracking().OrderBy(r => r.Name).Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name
            }).ToArrayAsync();
        }

        public async Task<(UpdateUserRolesDTO? Dto, string[] InvalidRoleIds, string[] Errors)> SetUserRolesAsync(string userId, IEnumerable<string> roleIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return (null, Array.Empty<string>(), new[] { "User not found." });

            HashSet<string> requestedIds = new HashSet<string>(roleIds ?? Array.Empty<string>());

            var requestedRoles = await _roleManager.Roles
                .Where(r => requestedIds.Contains(r.Id))
                .Select(r => new { r.Id, r.Name })
                .ToListAsync();

            HashSet<string> foundIds = requestedRoles.Select(r => r.Id).ToHashSet();
            string[] invalidIds = requestedIds.Except(foundIds).ToArray();

            HashSet<string> requested = requestedRoles.Select(r => r.Name!).ToHashSet(StringComparer.OrdinalIgnoreCase);
            HashSet<string> currentRoles = (await _userManager.GetRolesAsync(user)).ToHashSet(StringComparer.OrdinalIgnoreCase);

            string[] rolesToAdd = requested.Except(currentRoles).ToArray();
            string[] rolesToRemove = currentRoles.Except(requested).ToArray();

            if (rolesToAdd.Length == 0 && rolesToRemove.Length == 0)
            {
                UpdateUserRolesDTO dtoNoChange = new UpdateUserRolesDTO
                {
                    UserId = userId,
                    Added = Array.Empty<string>(),
                    Removed = Array.Empty<string>(),
                    CurrentRoles = currentRoles.OrderBy(n => n).ToArray()
                };
                return (dtoNoChange, invalidIds, Array.Empty<string>());
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            List<string> errors = new List<string>();

            if (rolesToAdd.Length > 0)
            {
                var addRes = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addRes.Succeeded) errors.AddRange(addRes.Errors.Select(e => e.Description));
            }

            if (rolesToRemove.Length > 0)
            {
                var remRes = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!remRes.Succeeded) errors.AddRange(remRes.Errors.Select(e => e.Description));
            }

            if (errors.Count > 0)
            {
                await transaction.RollbackAsync();
                return (null, invalidIds, errors.ToArray()); // critical fail
            }

            await transaction.CommitAsync();

            string[] finalRoles = (await _userManager.GetRolesAsync(user)).OrderBy(n => n).ToArray();

            UpdateUserRolesDTO dto = new UpdateUserRolesDTO
            {
                UserId = userId,
                Added = rolesToAdd,
                Removed = rolesToRemove,
                CurrentRoles = finalRoles
            };

            return (dto, invalidIds, Array.Empty<string>());
        }


    }
}
