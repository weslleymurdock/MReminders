namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface IBiometricsService
{
    Task<bool> AuthenticateAsync();
}