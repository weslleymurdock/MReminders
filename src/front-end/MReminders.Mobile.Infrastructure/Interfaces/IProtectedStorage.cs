namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface IProtectedStorage<T> where T : class
{
    string UserKey { get; }
    string UserId { get; }
    string ReminderId { get; }

    Task SetAsync(string key, T value);
    Task<T> GetAsync(string key);
}
