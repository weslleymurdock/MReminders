using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;



#if ANDROID
using AndroidX.Core.App;
using Android.Content.PM;
#endif
#if IOS
using Foundation;
using LocalAuthentication;
#endif

namespace MReminders.Mobile.Infrastructure.Services;

public class PermissionsService(ILogger<PermissionsService> logger) : IPermissionsService
{
    private TaskCompletionSource<bool> _tcs;
    public Task<bool> ValidateFingerPrintOrTouchIdPermissionsGranted()
    {
        try
        {
            logger.LogInformation($"{nameof(ValidateFingerPrintOrTouchIdPermissionsGranted)} starts");
#if ANDROID
            var permissionStatus = Platform.AppContext.CheckSelfPermission(Android.Manifest.Permission.UseBiometric);

            if (permissionStatus == Permission.Granted)
            {
                // Permissão já concedida
                return Task.FromResult(true);
            }

            // Registrar a callback
            _tcs = new TaskCompletionSource<bool>();
            ActivityCompat.RequestPermissions(Platform.CurrentActivity!, [Android.Manifest.Permission.UseBiometric], 0);

            // Aguardar até que a permissão seja concedida ou negada
            return _tcs.Task;
#elif IOS
            var context = new LAContext();
            return Task.FromResult(context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError authError));
#else
            throw new PlatformNotSupportedException();
#endif
        }
        catch (Exception e)
        {
            logger.LogInformation($"{nameof(ValidateFingerPrintOrTouchIdPermissionsGranted)} has errors");
            logger.LogError(e, e.Message);
            return Task.FromResult(false);
        }
        finally
        {
            logger.LogInformation($"{nameof(ValidateFingerPrintOrTouchIdPermissionsGranted)} ends");
        }
    }
#if ANDROID
    public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults) => _tcs.TrySetResult(requestCode == 0 && grantResults.Length > 0 && grantResults[0] == Permission.Granted);

    public async Task<bool> ValidateExternalStorageReadPermissionGranted()
    {
        try
        {
            return false;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

#endif
}
