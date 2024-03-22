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

        public AuthenticationService(AuthenticationStateProvider authStateProvider,
                                    UserManager<todoApp.Data.ApplicationUser> userManager,
                                    CPRServices cprServices,
                                    ToDoListServices toDoListServices)
        {
            _authStateProvider = authStateProvider;
            _userManager = userManager;
            _cprServices = cprServices;
            _toDoListServices = toDoListServices;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<string> GetUserEmailByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Email;
        }

        public async Task<string?> GetUserIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user?.Id;
        }

        public async Task<List<UserInfo>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .Select(u => new UserInfo { Id = u.Id, Email = u.Email })
                .ToListAsync();

            return users;
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
