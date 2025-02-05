#if IOS || MACCATALYST
using Foundation;
#endif
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Client.ViewModels;
using MReminders.Mobile.Client.Views;
using MReminders.Mobile.Infrastructure.Handlers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UraniumUI;

namespace MReminders.Mobile.Client;

public static class DependencyInjection
{

    public static MauiAppBuilder AddMauiBase(this MauiAppBuilder builder)
    {
        using var stream = typeof(DependencyInjection).Assembly.GetManifestResourceStream("MReminders.Mobile.Client.appsettings.json")!;
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(config);
        builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFontAwesomeIconFonts();
                });
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder;
    }
    public static MauiAppBuilder AddViewsAndViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<BaseViewModel>();
        builder.Services.AddTransient<AuthorizeViewModel>();
        builder.Services.AddTransient<LoginPage, LoginViewModel>();
        builder.Services.AddTransient<RegisterPage, RegisterViewModel>();
        builder.Services.AddTransient<MainPage, MainViewModel>();
        builder.Services.AddTransient<EditReminderPage, EditReminderViewModel>();
        builder.Services.AddTransient<ReminderPage, ReminderViewModel>();
        return builder;
    }

    public static MauiAppBuilder AddPopups(this MauiAppBuilder builder)
    {
        //builder.Services.AddTransientPopup<>();
        return builder;
    }

    public static MauiAppBuilder AddHandlers(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<IgnoreCertificateValidationHandler>();
        return builder;
    }

    public static MauiAppBuilder AddCertificate(this MauiAppBuilder builder)
    {

#if ANDROID
        builder.Services.AddTransient(sp =>
        {
            var context = Android.App.Application.Context;
            using var certStream = context.Assets!.Open("certificate.pem");
            using var keyStream = context.Assets.Open("pk.key");

            return sp.AddCertificateHandler(builder, certStream, keyStream);
        });
#endif

#if IOS || MACCATALYST
        builder.Services.AddTransient(sp =>
        {
            var certPath = Path.Combine(NSBundle.MainBundle.BundlePath, "Resources/Raw/certificate.pem");
            var keyPath = Path.Combine(NSBundle.MainBundle.BundlePath, "Resources/Raw/pk.key");

            return sp.AddCertificateHandler(builder, certPath, keyPath);
        });
#endif

#if WINDOWS
        builder.Services.AddTransient(sp =>
        {
            var certPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Resources/Raw/certificate.pem");
            var keyPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Resources/Raw/pk.key");

            return sp.AddCertificateHandler(builder, certPath, keyPath);
        });
#endif
        return builder;
    }
#if IOS || MACCATALYST || WINDOWS
    private static CertificateHandler AddCertificateHandler(this IServiceProvider _, MauiAppBuilder builder, string certPath, string keyPath)
    {
        var certBytes = File.ReadAllBytes(certPath);
        var keyBytes = File.ReadAllBytes(keyPath);
        var passString = builder.Configuration.GetRequiredSection("Certificate:Passphrase").Value;
        var certString = System.Text.Encoding.UTF8.GetString(certBytes);
        var keyString = System.Text.Encoding.UTF8.GetString(keyBytes);

        // Importar a chave privada usando o RSACryptoServiceProvider
        var rsa = RSA.Create();
        rsa.ImportFromEncryptedPem(keyString, passString);

        // Combinar a chave privada com o certificado
        var certWithKey = X509Certificate2.CreateFromPem(certString).CopyWithPrivateKey(rsa);

        return new CertificateHandler(certWithKey);
    }
#endif
#if ANDROID
    private static CertificateHandler AddCertificateHandler(this IServiceProvider _, MauiAppBuilder builder, Stream certStream, Stream keyStream)
    {
        using var certMemoryStream = new MemoryStream();
        certStream.CopyTo(certMemoryStream);
        var certBytes = certMemoryStream.ToArray();

        using var keyMemoryStream = new MemoryStream();
        keyStream.CopyTo(keyMemoryStream);
        var keyBytes = keyMemoryStream.ToArray(); 
        var passString = builder.Configuration.GetRequiredSection("Certificate:Passphrase").Value;
        var certString = System.Text.Encoding.UTF8.GetString(certBytes);
        var keyString = System.Text.Encoding.UTF8.GetString(keyBytes);

        // Importar a chave privada usando o RSACryptoServiceProvider
        var rsa = RSA.Create();
        rsa.ImportFromEncryptedPem(keyString, passString);

        // Combinar a chave privada com o certificado
        var certWithKey = X509Certificate2.CreateFromPem(certString).CopyWithPrivateKey(rsa);
 
        return new CertificateHandler(certWithKey);
    }
#endif
}
