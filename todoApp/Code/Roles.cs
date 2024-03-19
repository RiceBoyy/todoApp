using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using System.Data.Common;
using System.Runtime.CompilerServices;
using todoApp.Data;

namespace todoApp.Code
{
    public class Roles
    {
        public async Task CreateUserRole(string user, string role, IServiceProvider serviceProvider)
        {
            // call manager
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Data.ApplicationUser>>();

            // create role in role table
            bool UserRoleCheck = await roleManager.RoleExistsAsync(role);
            if (!UserRoleCheck)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            // add user in the role
            Data.ApplicationUser identityUser = await userManager.FindByEmailAsync(user);
            await userManager.AddToRoleAsync(identityUser, role);
        }
    }
}
