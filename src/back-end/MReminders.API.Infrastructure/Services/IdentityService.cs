using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.EntityFrameworkCore;
using MReminders.API.Infrastructure.Interfaces;
using MReminders.API.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MReminders.API.Infrastructure.Services;

public class IdentityService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, ILogger<IdentityService> logger) : IIdentityService
{
    public async Task<bool> ResetPasswordAsync(string userKey, string oldPassword, string newPassword)
    {
        try
        {
            var user = await FindUser(userKey);
            if (user == null)
            {
                return false; // Usuário não encontrado
            }

            // Verifica a senha antiga
            if (!await userManager.CheckPasswordAsync(user, oldPassword))
            {
                return false; // Senha antiga não corresponde
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resetting user password.");
            return false;
        }
    }

    public async Task<AppUser> FindUser(string key)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(key)
               ?? await userManager.FindByIdAsync(key)
               ?? await userManager.FindByNameAsync(key)
               ?? (await userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == key));
            return user ?? default!;
        }
        catch (Exception e)
        {
            return default!;
        }
    }

    public async Task<bool> AssignUserToRole(string key, IList<string> roles)
    {
        try
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == key || x.Email == key || x.PhoneNumber == key) ??
                throw new KeyNotFoundException("User not found");
            var result = await userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));

            throw;
        }
    }

    public async Task<bool> CreateRoleAsync(string roleName)
    {
        try
        {
            var result = await roleManager.CreateAsync(new AppRole() { Name = roleName, CreatedDate = DateTime.Now });
            if (!result.Succeeded)
            {
                throw new ValidationException(string.Join("\n ", result.Errors));
            }
            return result.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }


    // Return multiple value
    public async Task<(bool isSucceed, string userId)> CreateUserAsync(string userName, string password, string email, string fullName, string phoneNumber,  List<string> roles)
    {
        try
        {
            var user = new AppUser()
            {
                FullName = fullName,
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber, 
                PhoneNumberConfirmed = true,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new ValidationException(string.Join("\n ", result.Errors));
            }

            var addUserRole = await userManager.AddToRolesAsync(user, roles);
            if (!addUserRole.Succeeded)
            {
                throw new ValidationException(string.Join("\n ", addUserRole.Errors));
            }
            return (result.Succeeded && addUserRole.Succeeded, user.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<bool> DeleteRoleAsync(string roleId)
    {
        try
        {
            var roleDetails = await roleManager.FindByIdAsync(roleId) ?? throw new KeyNotFoundException("Role not found");
            if (roleDetails.Name == "SysAdmin" || roleDetails.Name == "Admin")
            {
                throw new InvalidOperationException("You can not delete Administrator Roles");
            }
           
            var result = await roleManager.DeleteAsync(roleDetails);
            if (!result.Succeeded)
            {
                throw new ValidationException(string.Join("\n ", result.Errors));
            }
            return result.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(string key)
    {
        try
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == key || x.Email == key || x.UserName == key || x.PhoneNumber == key) ?? throw new KeyNotFoundException("User not found");
            if (user.UserName?.ToLower() == "system" || user.UserName?.ToLower() == "admin" || user.UserName?.ToLower() == "root")
            {
                throw new Exception("You can not delete system or admin user");
                //throw new BadRequestException("You can not delete system or admin user");
            }
            var result = await userManager.DeleteAsync(user);
            return result.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<ICollection<AppUser>> GetAllUsersAsync()
    {
        try
        {
            var result = userManager.Users.ToList();
            
            return await Task.FromResult(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<List<(string id, string roleName)>> GetRoles()
    {
        try
        {
            var roles = await roleManager.Roles.Select(x => new
            {
                x.Id,
                x.Name
            }).ToListAsync();

            return roles.Select(role => (role.Id, role.Name)).ToList()!;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<(string userId, string fullName, string UserName, string email, string phone, IList<string> roles)> GetUserDetails(string key)
    {
        try
        {
            var user = userManager.Users.FirstOrDefault(x => x.Id == key || x.Email == key || x.UserName == key || x.PhoneNumber == key) ?? throw new KeyNotFoundException("User not found");
            var roles = await userManager.GetRolesAsync(user) ?? throw new KeyNotFoundException("Role not found"); ;
            return (user.Id, user.FullName, user.UserName, user.Email, user.PhoneNumber, roles)!;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

   
    public async Task<string> GetUserId(string key)
    {
        try
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == key || x.Email == key || x.PhoneNumber == key) ?? throw new KeyNotFoundException("User not found");
            return await userManager.GetUserIdAsync(user);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<string> GetUserName(string key)
    {
        try
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == key || x.Email == key || x.PhoneNumber == key) ?? throw new KeyNotFoundException("User not found");
            return (await userManager.GetUserNameAsync(user)) ?? string.Empty;
        }
        catch (Exception e)
        {
            logger.LogError(string.Format("exception: {0} stack trace: {1}", e.Message, e.StackTrace));
            return string.Empty;
        }
    }

    public async Task<List<string>> GetUserRoles(string key)
    {
        try
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == key || x.Email == key || x.UserName == key || x.PhoneNumber == key) ?? throw new KeyNotFoundException("User not found");
            var roles = await userManager.GetRolesAsync(user);
            return [.. roles];
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<bool> IsInRole(string key, string role)
    {
        try
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == key || x.Email == key || x.UserName == key || x.PhoneNumber == key) ?? throw new KeyNotFoundException("User not found");
            return await userManager.IsInRoleAsync(user, role);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<bool> IsUniqueUserName(string userName)
    {
        try
        {
            var user = await userManager.FindByNameAsync(userName) ?? new AppUser() { Id = "" };
            return user.Id == string.Empty;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }
    
    public async Task<bool> IsUniqueEmail(string email)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(email) ?? new AppUser() { Id = "" };
            return user.Id == string.Empty;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }
      
    public async Task<bool> IsUniquePhoneNumber(string number)
    {
        try
        {
            return !(await userManager.Users.AnyAsync(x => x.PhoneNumber == number));
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<bool> SigninUser(string userName, string password)
    {
        try
        {
            var result = await signInManager.PasswordSignInAsync(userName, password, true, false);
            return result.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<bool> UpdateUserProfileAsync(string id, string fullName, string email, string phoneNumber, IList<string> roles)
    {
        try
        {
            var user = await userManager.FindByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
            user.FullName = fullName;
            user.Email = email;
            user.PhoneNumber = phoneNumber;
            var result = await userManager.UpdateAsync(user);

            return result.Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

 
    public async Task<(string id, string roleName)> GetRoleById(string id)
    {
        try
        {
            var role = await roleManager.FindByIdAsync(id) ?? throw new KeyNotFoundException("Role not found"); ;
            return (role.Id, role.Name)!;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<bool> UpdateRole(string id, string roleName)
    {
        try
        {
            if (roleName != null)
            {
                var role = await roleManager.FindByIdAsync(id) ?? throw new KeyNotFoundException("Role not found");
                role.Name = roleName;
                var result = await roleManager.UpdateAsync(role);
                return result.Succeeded;
            }
            return false;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }

    public async Task<bool> UpdateUsersRole(string userName, IList<string> usersRole)
    {
        try
        {
            var user = await userManager.FindByNameAsync(userName) ?? throw new KeyNotFoundException("User not found");
             
            var existingRoles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, existingRoles);
            _ = await userManager.AddToRolesAsync(user, usersRole);

            return (await userManager.RemoveFromRolesAsync(user, existingRoles)).Succeeded;
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Concat("exception: ", e.Message, " stack trace: ", e.StackTrace));
            throw;
        }
    }
}