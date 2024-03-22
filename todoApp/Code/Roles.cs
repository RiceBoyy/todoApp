using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using todoApp.Data; // Ensure this uses your actual namespace

public class Roles
{
    public async Task CreateUserRole(string userId, string role, IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Check if the role exists, create if not
        bool roleExists = await roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Find user by ID and add them to the role
        ApplicationUser appUser = await userManager.FindByIdAsync(userId);
        if (appUser != null)
        {
            await userManager.AddToRoleAsync(appUser, role);
        }
    }

    public async Task RemoveUserRole(string userId, string role, IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Find user by ID
        ApplicationUser appUser = await userManager.FindByIdAsync(userId);
        if (appUser != null)
        {
            // Remove user from the role
            await userManager.RemoveFromRoleAsync(appUser, role);
        }
    }
}
