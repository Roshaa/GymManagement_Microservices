using Microsoft.AspNetCore.Identity;

public class IdentitySeeder
{
    public async Task SeedAsync(IServiceProvider sp)
    {
        var roles = new[] { "Admin", "Administrativo", "Personal Trainer" };

        var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var r in roles)
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        var userMgr = sp.GetRequiredService<UserManager<IdentityUser>>();

        await EnsureAdminAsync(userMgr, "admin1@gym.local", "Admin123$", rolesToAdd: new[] { "Admin" });
        await EnsureAdminAsync(userMgr, "admin2@gym.local", "Admin123$", rolesToAdd: new[] { "Admin" });
    }

    private async Task EnsureAdminAsync(
        UserManager<IdentityUser> userMgr,
        string email,
        string password,
        string[] rolesToAdd)
    {
        var user = await userMgr.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            var create = await userMgr.CreateAsync(user, password);
            if (!create.Succeeded)
                throw new InvalidOperationException($"Failed creating seed user {email}: {string.Join(", ", create.Errors.Select(e => e.Description))}");
        }

        foreach (var role in rolesToAdd)
            if (!await userMgr.IsInRoleAsync(user, role))
                await userMgr.AddToRoleAsync(user, role);
    }
}
