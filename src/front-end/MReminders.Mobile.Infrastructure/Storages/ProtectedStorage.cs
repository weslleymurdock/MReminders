#if IOS || MACCATALYST
using Foundation;
using Security;
#endif
using MReminders.Mobile.Infrastructure.Interfaces;
using System.Text.Json;

namespace MReminders.Mobile.Infrastructure.Storages;

public class ProtectedStorage<T> : IProtectedStorage<T> where T : class
{

    public string UserKey { get => "UserKey"; }

    public string UserId { get => "UserId"; }

    public string ReminderId { get => "ReminderId"; }

    public async Task SetAsync(string key, T value)
    {
        string json = string.Empty;
        await Task.Run(() =>
        {
            json = JsonSerializer.Serialize(value);
        });
#if __ANDROID__
        await SecureStorage.SetAsync(key, json);
#elif WINDOWS
        await Task.Run(() => { Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = json; });
#elif MACCATALYST || IOS
        await Task.Run(() =>
    {
        var record = new SecRecord(SecKind.GenericPassword)
        {
            Service = "mreminders",
            Account = key,
            ValueData = NSData.FromString(json, NSStringEncoding.UTF8)
        };

        SecStatusCode status = SecKeyChain.Add(record);

        // Handle existing record case
        if (status == SecStatusCode.DuplicateItem)
        {
            SecKeyChain.Remove(record);
            status = SecKeyChain.Add(record);
        }

        if (status != SecStatusCode.Success)
            throw new Exception("Erro ao salvar no Keychain: " + status);
    });
#else
        throw new PlatformNotSupportedException();
#endif
    }


    public async Task<T> GetAsync(string key)
    {
        await Task.Delay(1);
#if __ANDROID__
        return await SecureStorage.GetAsync(key) is string json ? JsonSerializer.Deserialize<T>(json)! : default!;
#elif WINDOWS
        return await Task.Run(() => Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] is string json ? JsonSerializer.Deserialize<T>(json)! : default!);
    
#elif MACCATALYST || IOS
        var query = new SecRecord(SecKind.GenericPassword)
        {
            Service = "mreminders",
            Account = key
        };

        SecStatusCode status;
        var match = SecKeyChain.QueryAsRecord(query, out status);

        if (status == SecStatusCode.Success && match != null && match.ValueData != null)
        {
            var json = NSString.FromData(match.ValueData, NSStringEncoding.UTF8);
            return JsonSerializer.Deserialize<T>(json!.ToString())!;
        }

        throw new InvalidOperationException("Reading error");
#else
        throw new PlatformNotSupportedException();
#endif


    }


}
