using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using todoApp.Code;
using todoApp.Models;

public class CPRServices
{
    private readonly TodoContext _context;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly UserManager<todoApp.Data.ApplicationUser> _userManager;
    private readonly HashingHandler _hashingHandler;

    public CPRServices(TodoContext context, AuthenticationStateProvider authStateProvider, UserManager<todoApp.Data.ApplicationUser> userManager, HashingHandler hashingHandler)
    {
        _context = context;
        _authStateProvider = authStateProvider;
        _userManager = userManager;
        _hashingHandler = hashingHandler;
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
        string encryptedCprNumber = _hashingHandler.PBKDF2_Hashing(cprNumber, userId);

        if (existingCpr == null)
        {
            _context.Cprs.Add(new CPR { User = userId, CPRNr = encryptedCprNumber });
            // _context.Cprs.Add(new CPR { User = userId, CPRNr = EncryptCprNumber(cprNumber) });

            await _context.SaveChangesAsync();
            return "CPR number added successfully.";
        }
        else
        {
            existingCpr.CPRNr = encryptedCprNumber;
            await _context.SaveChangesAsync();
            return "CPR number updated successfully.";
        }
    }
    public async Task<CPR> GetUserCprByEmailAsync(string email)
    {
        // Fetch the user ID associated with the given email.
        var userId = await GetUserIdByEmailAsync(email);

        // If a user ID is found, fetch the corresponding CPR record.
        if (!string.IsNullOrEmpty(userId))
        {
            var userCpr = await _context.Cprs.FirstOrDefaultAsync(u => u.User == userId);
            return userCpr;
        }
        // If no user ID is found, or no corresponding CPR record exists, return null.
        return null;
    }
    public async Task<List<CPR>> FilterUserIdByNumberAsync(string UserId)
    {
        return await _context.Cprs
                             .Where(c => c.User == UserId)
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
    public async Task<bool> HandleCprSubmitAsync(string cprNumber)
    {
        var userId = await GetCurrentUserIdAsync();
        var existingCpr = await _context.Cprs.FirstOrDefaultAsync(c => c.User == userId);

        if (existingCpr != null)
        {
            // Verify the input against the stored hash
            string inputCprHash = _hashingHandler.PBKDF2_Hashing(cprNumber, userId);
            return existingCpr.CPRNr.Equals(inputCprHash);
        }
        else
        {
            // If no existing CPR number, hash the input and store it
            string encryptedCprNumber = _hashingHandler.PBKDF2_Hashing(cprNumber, userId);
            _context.Cprs.Add(new CPR { User = userId, CPRNr = encryptedCprNumber });
            await _context.SaveChangesAsync();
            return true; // New CPR added successfully
        }
    }


    private async Task<string> GetCurrentUserIdAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
