using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Account;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Application.Responses.Reminders;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Requests.Reminders;

public class GetUserAccountRequest : IRequest<BaseResponse<UserResponse>>
{
    public string UserKey { get; set; } = string.Empty;
}

public class GetUserAccountRequestHandler(IIdentityService identity, IMapper mapper, ILogger<GetUserAccountRequestHandler> logger) : IRequestHandler<GetUserAccountRequest, BaseResponse<UserResponse>>
{
    public async Task<BaseResponse<UserResponse>> Handle(GetUserAccountRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await identity.FindUser(request.UserKey);
            if (user is null)
            {
                return new BaseResponse<UserResponse>
                {
                    Data = { },
                    Message = "User not found",
                    StatusCode = 404,
                    Success = false
                };
            }
            var response = mapper.Map<UserResponse>(user) ?? throw new InvalidOperationException("there was an error when maping userresponse");
            var result = response is not null && response != default!;
            return new BaseResponse<UserResponse>
            {
                Data = response!,
                Message = "Reminders not found",
                StatusCode = result ? 200 : 400,
                Success =  result
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}

