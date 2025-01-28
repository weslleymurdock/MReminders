using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Application.Responses.Reminders;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Requests.Reminders;

public class GetRemindersFromUserRequest : IRequest<BaseResponse<IEnumerable<ReminderResponse>>>
{
    public string UserKey { get; set; } = string.Empty;
}

public class GetRemindersFromUserRequestHandler(IReminderService service, IIdentityService identity, IMapper mapper, ILogger<GetRemindersFromUserRequestHandler> logger) : IRequestHandler<GetRemindersFromUserRequest, BaseResponse<IEnumerable<ReminderResponse>>>
{
    public async Task<BaseResponse<IEnumerable<ReminderResponse>>> Handle(GetRemindersFromUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await identity.GetUserId(request.UserKey);
            if (string.IsNullOrEmpty(userId))
            {
                return new BaseResponse<IEnumerable<ReminderResponse>>
                {
                    Data = default!,
                    Message = "User not found",
                    StatusCode = 404,
                    Success = false
                };
            }
            var response = service.GetReminders().Where(x => x.UserId == userId);
            if (response.Any())
            {
                return new()
                {
                    Data = response.Select(x => mapper.Map<ReminderResponse>(x)).ToList(),
                    Message = "Ok",
                    Success = true,
                    StatusCode = 200
                };
            }
            return new BaseResponse<IEnumerable<ReminderResponse>>
            {
                Data = default!,
                Message = "Reminders not found",
                StatusCode = 404,
                Success = false
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new BaseResponse<IEnumerable<ReminderResponse>>
            {
                Data = default!,
                Message = $"Error when getting reminders: {e.Message}",
                StatusCode = 400,
                Success = false
            };
        }
    }
}

