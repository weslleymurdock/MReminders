using MReminders.Mobile.Infrastructure.Interfaces;

namespace MReminders.Mobile.Infrastructure.Services;


#if IOS
    using LocalAuthentication;
    using Foundation;

    public class BiometricsService : IBiometricsService
    {
        public async Task<bool> AuthenticateAsync()
        {
            var context = new LAContext();
            NSError authError;
            var myReason = new NSString("To access your secure data");

            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out authError))
            {
                var result = await context.EvaluatePolicyAsync(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, myReason);
                return result.Item1;
            }

            return false;
        }
    }
#elif ANDROID 
using Android.Content;
using Android.Hardware.Biometrics;
using Android.Hardware.Fingerprints;
using Android.OS;
using Android.Security.Keystore;
using Java.Security;
using Java.Util.Concurrent;
using Javax.Crypto;
using Task = System.Threading.Tasks.Task;

public class BiometricsService : IBiometricsService
{
    public async Task<bool> AuthenticateAsync()
    {
        var executor = Executors.NewSingleThreadExecutor()!;
        var cancelSignal = new CancellationSignal();
        var callback = new SimpleAuthenticationCallback();
        var charSequence = new Java.Lang.String("Cancel");
        var negativeButtonListener = new DialogOnClickListener(() => cancelSignal.Cancel());
        var biometricPrompt = new BiometricPrompt.Builder(Platform.CurrentActivity!.ApplicationContext)
            .SetTitle("Biometric Authentication")
            .SetDescription("To access your secure data")
            .SetNegativeButton(charSequence, executor, negativeButtonListener)
            .Build();
        var cryptoObject = GenerateCryptoObject();
        biometricPrompt.Authenticate(new BiometricPrompt.CryptoObject(cryptoObject), cancelSignal, executor, callback);
        return await callback.Task;
    }
    private Cipher GenerateCryptoObject()
    {
        // Carregar ou criar uma chave de criptografia
        var keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, "AndroidKeyStore")!;
        var keyStore = KeyStore.GetInstance("AndroidKeyStore")!;
        keyStore.Load(null);

        var keyGenParameterSpec = new KeyGenParameterSpec.Builder(
            "biometrics_key",
            KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
            .SetBlockModes(KeyProperties.BlockModeCbc)
            .SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7)
            .Build();

        keyGenerator.Init(keyGenParameterSpec);
        var secretKey = keyGenerator.GenerateKey();

        // Inicializar o Cipher
        var cipher = Cipher.GetInstance($"{KeyProperties.KeyAlgorithmAes}/{KeyProperties.BlockModeCbc}/{KeyProperties.EncryptionPaddingPkcs7}")!;
        cipher.Init(CipherMode.EncryptMode, secretKey);
        return cipher;
    }

 
    public class SimpleAuthenticationCallback : BiometricPrompt.AuthenticationCallback
    {
        private readonly TaskCompletionSource<bool> _tcs = new();

        public Task<bool> Task => _tcs.Task;

        public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult? result)
        {
            _tcs.SetResult(true);
        }

        public override void OnAuthenticationFailed()
        {
            _tcs.SetResult(false);
        }
    }
    private class DialogOnClickListener(Action action) : Java.Lang.Object, IDialogInterfaceOnClickListener
    {
        public void OnClick(IDialogInterface? dialog, int which) => action.Invoke();
    }
}
public static class BiometricPromptExtensions
{
    public static  Task<bool> AuthenticateAsync(this BiometricPrompt biometricPrompt, BiometricPrompt.CryptoObject crypto, Android.OS.CancellationSignal cancellationSignal, IExecutor executor)
    {
        var callback = new BiometricsService.SimpleAuthenticationCallback();
        biometricPrompt.Authenticate(crypto, cancellationSignal, executor, callback);
        return callback.Task;
    }
}
#else
    public class BiometricsService : IBiometricsService
    {
        public Task<bool> AuthenticateAsync()
        {
            throw new NotImplementedException("Biometric authentication is not implemented for this platform.");
        }
    }
#endif