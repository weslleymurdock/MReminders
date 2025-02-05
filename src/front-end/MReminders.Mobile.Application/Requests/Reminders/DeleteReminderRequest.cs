using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Reminders;

public class DeleteReminderRequest : IRequest<BooleanBaseResponse>
{
    public string Id { get; set; } = string.Empty;
}
public class DeleteReminderRequestHandler(IRemindersService service, ILogger<DeleteReminderRequestHandler> logger) : IRequestHandler<DeleteReminderRequest, BooleanBaseResponse>
{
    public async Task<BooleanBaseResponse> Handle(DeleteReminderRequest request, CancellationToken cancellationToken)
    {
		try
		{
			var _request = new Rest.Client.DeleteReminderRequest()
			{
				ReminderId = request.Id
			};

			return await service.DeleteReminderAsync(_request, cancellationToken);
		}
		catch (Exception e)
		{
			logger.LogError(e, e.Message);
			throw;
		}
    }
}
