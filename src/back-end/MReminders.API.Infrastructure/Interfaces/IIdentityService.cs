using MReminders.API.Domain.Identity;

namespace MReminders.API.Infrastructure.Interfaces;

public interface IIdentityService
{
    // User section
    Task<bool> ResetPasswordAsync(string userKey, string oldPassword, string newPassword);
    Task<AppUser> FindUser(string key);
    Task<(bool isSucceed, string userId)> CreateUserAsync(string userName, string password, string email, string fullName, string phoneNumber, List<string> roles );
    Task<bool> SigninUser(string userName, string password);
    Task<string> GetUserId(string key);
    Task<(string userId, string fullName, string UserName, string email, string phone, IList<string> roles)> GetUserDetails(string key); 
    Task<string> GetUserName(string key);
    Task<bool> DeleteUserAsync(string key);
    Task<bool> IsUniqueUserName(string userName); 
    Task<bool> IsUniquePhoneNumber(string number);
    Task<bool> IsUniqueEmail(string email);
    Task<ICollection<AppUser>> GetAllUsersAsync();
    Task<bool> UpdateUserProfileAsync(string id, string fullName, string email, string phoneNumber, IList<string> roles);
     
    // Role Section
    Task<bool> CreateRoleAsync(string roleName);
    Task<bool> DeleteRoleAsync(string roleId);
    Task<List<(string id, string roleName)>> GetRoles();
    Task<(string id, string roleName)> GetRoleById(string id);
    Task<bool> UpdateRole(string id, string roleName);

    // User's Role section
    Task<bool> IsInRole(string key, string role);
    Task<List<string>> GetUserRoles(string key);
    Task<bool> AssignUserToRole(string key, IList<string> roles);
    Task<bool> UpdateUsersRole(string key, IList<string> usersRole);

}