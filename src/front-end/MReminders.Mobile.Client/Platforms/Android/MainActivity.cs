using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using MReminders.Mobile.Client.Messages;
using MReminders.Mobile.Infrastructure.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace MReminders.Mobile.Client
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public required IPermissionsService _permissionsService;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _permissionsService = Shell.Current.Handler!.MauiContext!.Services.GetRequiredService<IPermissionsService>();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            _permissionsService.OnRequestPermissionsResult(requestCode, permissions, grantResults);
#pragma warning disable CA1416 // Validar a compatibilidade da plataforma
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
#pragma warning restore CA1416 // Validar a compatibilidade da plataforma
        }



        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 0 && resultCode == Result.Ok)
            {
                var uri = data!.Data!;
                string filePath = GetFilePathFromUri(Android.App.Application.Context, uri);


                WeakReferenceMessenger.Default.Send(new FilePathMessage(filePath));
            }
        }
        public Stream GetInputStreamFromUri(Context context, Android.Net.Uri uri)
        {
            return context.ContentResolver.OpenInputStream(uri)!;
        }

        public string CopyStreamToFile(Context context, Stream inputStream, Android.Net.Uri uri)
        {
            string fileName = GetFileNameFromUri(Android.App.Application.Context, uri);

            using var outputStream = new FileStream(Path.Combine(context.CacheDir!.AbsolutePath, fileName), FileMode.Create, FileAccess.Write);
            {
                inputStream.CopyTo(outputStream);
            }

            return Path.Combine(context.CacheDir.AbsolutePath, fileName); ;
        }
        public string GetFilePathFromUri(Context context, Android.Net.Uri uri)
        {
            using var inputStream = GetInputStreamFromUri(context, uri);

            if (inputStream != null)
            {
                return CopyStreamToFile(context, inputStream, uri);
            }

            return string.Empty;
        }
        public string GetFileNameFromUri(Context context, Android.Net.Uri uri)
        {
            string fileName = null;
            var cursor = context.ContentResolver.Query(uri, null, null, null, null);
            if (cursor != null && cursor.MoveToFirst())
            {
                int nameIndex = cursor.GetColumnIndex(OpenableColumns.DisplayName);
                if (nameIndex != -1)
                {
                    fileName = cursor.GetString(nameIndex);
                }
                cursor.Close();
            }
            return fileName;
        }
    }
}
