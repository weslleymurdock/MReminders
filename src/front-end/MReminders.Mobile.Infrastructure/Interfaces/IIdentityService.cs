using MReminders.Mobile.Domain.Entities;
using MReminders.Rest.Client;
using System.Linq.Expressions;

namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface IIdentityService
{
    Task<UserResponseBaseResponse> Register(AppUser appUser, string password, string[] roles); 
    Task SetBearer(string bearer);
    Task<UserResponse> GetProfile(string key);
    Task<BooleanBaseResponse> UpdateProfile(EditAccountProfileRequest request);
}
