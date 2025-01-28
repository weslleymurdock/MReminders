using DnsClient.Internal;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Requests.Account;

public class DeleteAccountRequest : IRequest<BaseResponse<bool>>
{
    public string Key { get; set; } = string.Empty;
}

public class DeleteAccountRequestHandler(IIdentityService identity, IValidator<DeleteAccountRequest> validator, ILogger<DeleteAccountRequestHandler> logger) : IRequestHandler<DeleteAccountRequest, BaseResponse<bool>>
{
    public async Task<BaseResponse<bool>> Handle(DeleteAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                return new BaseResponse<bool>() { Data = false, Message = string.Join("\n ", validation.Errors), StatusCode = 422, Success = false };
            }

            var result = await identity.DeleteUserAsync(request.Key);

            return new BaseResponse<bool>() { Success = result, StatusCode = result ? 200 : 400, Data = result, Message = result ? "Ok" : "A error occurred when deleting user" };
        }
        catch (Exception e)
        {
            return new BaseResponse<bool>() { Data = false, Message = $"A error ocurred when deleting user: {e.Message}", StatusCode = 400, Success = false };
        }
    }
}
