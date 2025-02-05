#if ANDROID
using Android.Content.PM;
#endif
namespace MReminders.Mobile.Infrastructure.Interfaces;

public interface IPermissionsService 
{
    Task<bool> ValidateFingerPrintOrTouchIdPermissionsGranted();
#if ANDROID
    void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults);
    Task<bool> ValidateExternalStorageReadPermissionGranted();
#endif
}
