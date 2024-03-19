using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;
using System.Security.Claims;
using todoApp.Models;

public class CPRServices
{
    private readonly TodoContext _context;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly UserManager<todoApp.Data.ApplicationUser> _userManager;

    public CPRServices(TodoContext context, AuthenticationStateProvider authStateProvider, UserManager<todoApp.Data.ApplicationUser> userManager)
    {
        _context = context;
        _authStateProvider = authStateProvider;
        _userManager = userManager;
    }

    public async Task<CPR> GetUserCprAsync()
    {
        var userId = await GetCurrentUserIdAsync();
        return await _context.Cprs.FirstOrDefaultAsync(c => c.User == userId);
    }



    // admin mode

    public async Task<string> HandleCprOnEmailSubmitAsync(string userEmail, string cprNumber)
    {
        var userId = await GetUserIdByEmailAsync(userEmail);
        if (userId == null)
        {
            return "User not found.";
        }

        var existingCpr = await _context.Cprs.FirstOrDefaultAsync(c => c.User == userId);
        if (existingCpr == null)
        {
            _context.Cprs.Add(new CPR { User = userId, CPRNr = cprNumber });
            await _context.SaveChangesAsync();
            return "CPR number added successfully.";
        }
        else
        {
            existingCpr.CPRNr = cprNumber;
            await _context.SaveChangesAsync();
            return "CPR number updated successfully.";
        }
    }

    public async Task<CPR> GetUserCprByEmailAsync(string email)
    {
        var user = await _context.Cprs.FirstOrDefaultAsync(u => u.User == email);
        return user != null ? await _context.Cprs.FirstOrDefaultAsync(c => c.User == user.Id.ToString()) : null;
    }

    public async Task<List<CPR>> FilterCprsByNumberAsync(string cprNumber)
    {
        return await _context.Cprs
                             .Where(c => c.CPRNr == cprNumber)
                             .ToListAsync();
    }

    public async Task<string?> GetUserIdByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user?.Id;
    }

    public async Task<List<CPR>> GetAllCprsAsync()
    {
        return await _context.Cprs.ToListAsync();
    }

    public async Task<bool> DeleteCprAsync(CPR cpr)
    {
        if (cpr == null) return false;

        try
        {
            _context.Cprs.Remove(cpr);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            // Handle any exceptions, such as logging
            return false;
        }
    }


    // User Mode

    public async Task<string> HandleCprSubmitAsync(string cprNumber)
    {
        var userId = await GetCurrentUserIdAsync();
        var existingCpr = await _context.Cprs.FirstOrDefaultAsync(c => c.User == userId);

        if (existingCpr == null)
        {
            _context.Cprs.Add(new CPR { User = userId, CPRNr = cprNumber });
            await _context.SaveChangesAsync();
            return "CPR number added successfully.";
        }
        else
        {
            existingCpr.CPRNr = cprNumber;
            await _context.SaveChangesAsync();
            return "CPR number updated successfully.";
        }
    }

    private async Task<string> GetCurrentUserIdAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    

}
