using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Infrastructure.Services;

public class TokenRenewalService(IIdentityService identityService, ITokenStorage tokenStorageService) : ITokenRenewalService
{ 
    private readonly CancellationTokenSource cancellationTokenSource = new ();
     

    public void StartTokenMonitoring()
    {
        _ = MonitorTokenAsync(cancellationTokenSource.Token);
    }

    public void StopTokenMonitoring()
    {
        cancellationTokenSource.Cancel();
    }

    private async Task MonitorTokenAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var token = await tokenStorageService.GetTokenAsync(Domain.Enums.TokenKind.Bearer);
            var remainingTime = token.Expires - DateTime.UtcNow;
            var delay = remainingTime.TotalMilliseconds - TimeSpan.FromMinutes(1).TotalMilliseconds;

            if (delay > 0)
            {
                await Task.Delay((int)delay, cancellationToken);

                if (DateTime.UtcNow >= token.Expires)
                {
                    await identityService.SetBearer((await tokenStorageService.GetTokenAsync(Domain.Enums.TokenKind.Basic)).Value); 
                }
            }
        }
    }
}
