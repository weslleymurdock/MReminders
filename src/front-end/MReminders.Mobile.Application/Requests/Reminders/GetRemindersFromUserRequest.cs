using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Reminders;

public class GetRemindersFromUserRequest : IRequest<ReminderResponseIEnumerableBaseResponse>
{
    public string Key { get; set; } = string.Empty;
}

public class GetRemindersFromUserRequestHandler(IRemindersService service, ILogger<GetRemindersFromUserRequestHandler> logger) : IRequestHandler<GetRemindersFromUserRequest, ReminderResponseIEnumerableBaseResponse>
{
    public async Task<ReminderResponseIEnumerableBaseResponse> Handle(GetRemindersFromUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation($"{nameof(GetRemindersFromUserRequest)} handle started");

            return await service.GetRemindersFromUserAsync(request.Key, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(GetRemindersFromUserRequest)} handle ended");
        }
    }
}