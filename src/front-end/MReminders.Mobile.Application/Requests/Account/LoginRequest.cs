using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Account;

public class LoginRequest : IRequest<LoginResponseBaseResponse>
{
    public string Key { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class LoginRequestHandler(MRemindersClient client, ITokenStorage tokenStorage, IProtectedStorage<string> protectedStorage, ITokenRenewalService renewalService, IValidator<LoginRequest> validator, ILogger<LoginRequestHandler> logger) : IRequestHandler<LoginRequest, LoginResponseBaseResponse>
{
    public async Task<LoginResponseBaseResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
            {
                return new LoginResponseBaseResponse()
                {
                    Data = null,
                    Message = string.Join("\n ", result.Errors),
                    StatusCode = int.TryParse(result.Errors.First().ErrorCode, out int code) ? code : 422,
                    Success = false
                };
            }
            var basic = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{request.Key}:{request.Password}"));
            client.BasicToken = basic;
            var response = await client.LoginAsync(cancellationToken);
            if (response.Success)
            {
                await tokenStorage.SaveTokenAsync(new Domain.Entities.Token { Expires = DateTime.MaxValue, Id = new Guid().ToString(), Kind = Domain.Enums.TokenKind.Basic, Value = basic });
                await protectedStorage.SetAsync(protectedStorage.UserKey, response.Data.Email);
                await protectedStorage.SetAsync(protectedStorage.UserId, response.Data.Id);
                await tokenStorage.SaveTokenAsync(new Domain.Entities.Token() { Expires = response.Data.ExpirationDate, Id = new Guid().ToString(), Kind = Domain.Enums.TokenKind.Bearer, Value = response.Data.Token });
                renewalService.StartTokenMonitoring();
            }
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
