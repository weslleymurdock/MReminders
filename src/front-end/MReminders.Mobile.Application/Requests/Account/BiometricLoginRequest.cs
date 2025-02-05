using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;
namespace MReminders.Mobile.Application.Requests.Account;

public class BiometricLoginRequest : IRequest<LoginResponseBaseResponse>
{
    public string BasicToken { get; set; } = string.Empty; 
}


public class BiometricLoginRequestHandler(IPermissionsService permissions, IBiometricsService biometrics, ITokenStorage tokenStorage, IProtectedStorage<string> stringStorage, MRemindersClient client, ILogger<BiometricLoginRequestHandler> logger) : IRequestHandler<BiometricLoginRequest, LoginResponseBaseResponse>
{
    public async Task<LoginResponseBaseResponse> Handle(BiometricLoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            bool isPermissionGranted = false;

            isPermissionGranted = await permissions.ValidateFingerPrintOrTouchIdPermissionsGranted();

            if (!isPermissionGranted)
            {
                return new LoginResponseBaseResponse()
                {
                    Data = null!,
                    Message = "O uso da biometria necessita de permissões",
                    StatusCode = 400,
                    Success = false
                };
            }


            if (!await biometrics.AuthenticateAsync())
            {
                return new LoginResponseBaseResponse()
                {
                    Data = null!,
                    Message = "Não autorizado",
                    StatusCode = 401,
                    Success = false
                };
            }
             var basic = request.BasicToken;
            if (string.IsNullOrEmpty(basic))
            {
                return new LoginResponseBaseResponse()
                {
                    Data = null!,
                    Message = "Necessário autenticar o usuario utilizando senha para habilitar autenticação biometrica",
                    StatusCode = 400,
                    Success = false
                };
            }

            var credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(basic ?? string.Empty)).Split(':', 2);
            client.BasicToken = request.BasicToken;
            var response = await client.LoginAsync(cancellationToken);
            if (response.Success)
            {
                await tokenStorage.SaveTokenAsync(new Domain.Entities.Token()
                {
                    Expires = response.Data.ExpirationDate,
                    Id = Guid.NewGuid().ToString(),
                    Kind = Domain.Enums.TokenKind.Bearer,
                    Value = response.Data.Token
                });
                await stringStorage.SetAsync(stringStorage.UserId, response.Data.Id);
                await stringStorage.SetAsync(stringStorage.UserKey, response.Data.Email);
            }

            return new LoginResponseBaseResponse()
            {
                Data = response.Data,
                Message = response.Message,
                Success = response.Success,
                StatusCode = response.StatusCode,
            };


        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
