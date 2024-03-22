using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using todoApp.Code;
using todoApp.Models;

namespace todoApp.Code
{
    public class AuthenticationService
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly UserManager<todoApp.Data.ApplicationUser> _userManager;
        private readonly CPRServices _cprServices;
        private readonly ToDoListServices _toDoListServices;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationService(AuthenticationStateProvider authStateProvider,
                                    UserManager<todoApp.Data.ApplicationUser> userManager,
                                    CPRServices cprServices,
                                    ToDoListServices toDoListServices,
                                    RoleManager<IdentityRole> roleManager)
        {
            _authStateProvider = authStateProvider;
            _userManager = userManager;
            _cprServices = cprServices;
            _toDoListServices = toDoListServices;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> DeleteAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var errors = new List<IdentityError>();

            foreach (var user in users)
            {
                // Delete associated ToDo items
                await _toDoListServices.DeleteAllTodoItemsByUserIdAsync(user.Id);

                // Delete associated CPR records
                var cprs = await _cprServices.FilterUserIdByNumberAsync(user.Id);
                foreach (var cpr in cprs)
                {
                    await _cprServices.DeleteCprAsync(cpr);
                }

                // Delete the user
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors); // Collect any errors encountered
                }
            }

            // Return aggregated result
            return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }



        public async Task EnsureAdminUserAsync()
        {
            var adminRoleName = "Admin";

            var admins = await _userManager.GetUsersInRoleAsync(adminRoleName);
            if (admins.Any())
            {
                return;
            }

            var user = await _userManager.Users.FirstOrDefaultAsync();
            if (user == null)
            {
                return;
            }

            if (!await _roleManager.RoleExistsAsync(adminRoleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            var result = await _userManager.AddToRoleAsync(user, adminRoleName);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to promote user {user.UserName} to Admin: {result.Errors.FirstOrDefault()?.Description}");
            }
        }


        public async Task<IdentityResult> DeleteAllNormalUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var errors = new List<IdentityError>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin") || roles.Contains("SuperUser"))
                {
                    continue; // Skip admin and super users
                }

                // Attempt to delete associated ToDo items
                await _toDoListServices.DeleteAllTodoItemsByUserIdAsync(user.Id);

                // Attempt to delete associated CPR records
                var cprs = await _cprServices.FilterUserIdByNumberAsync(user.Id);
                foreach (var cpr in cprs)
                {
                    await _cprServices.DeleteCprAsync(cpr);
                }

                // Finally, attempt to delete the user
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors); // Collect any errors
                }
            }

            return errors.Any() ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }


        public async Task<IdentityResult> DeleteUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"User with ID {userId} not found." });
            }

            await _toDoListServices.DeleteAllTodoItemsByUserIdAsync(userId);

            var cprs = await _cprServices.FilterUserIdByNumberAsync(userId);
            foreach (var cpr in cprs)
            {
                await _cprServices.DeleteCprAsync(cpr);
            }

            return await _userManager.DeleteAsync(user);
        }

        public class UserInfo
        {
            public string Id { get; set; }
            public string Email { get; set; }
        }
    }
}
