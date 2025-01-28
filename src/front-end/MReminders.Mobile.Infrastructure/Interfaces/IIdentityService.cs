using MReminders.Mobile.Domain.Entities;

namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface IIdentityService
{
    Task<bool> SignInAsync(string username, string password);
    Task<bool> Register(AppUser appUser);
    Task<bool> ResetPassword(AppUser appUser);

    Task<bool> SetBiometrics(string userId, BiometricData)
}
