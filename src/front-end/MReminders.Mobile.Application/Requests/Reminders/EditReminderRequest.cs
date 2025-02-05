using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Reminders;

public class EditReminderRequest : IRequest<ReminderResponseBaseResponse>
{
    public DateTime DueDate { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; }= string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ReminderId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class EditReminderRequestHandler(IRemindersService service, IValidator<EditReminderRequest> validator, ILogger<EditReminderRequestHandler> logger) : IRequestHandler<EditReminderRequest, ReminderResponseBaseResponse>
{
    public async Task<ReminderResponseBaseResponse> Handle(EditReminderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                return new()
                {
                    Data = null,
                    Message = string.Join("\n ", validation.Errors),
                    Success = false
                };
            }
            Rest.Client.EditReminderRequest _request = new Rest.Client.EditReminderRequest() { Description = request.Description, DueDate = request.DueDate, Location = request.Location, Name = request.Name, UserId = request.UserId, Id = request.ReminderId, Done = request.DueDate <= DateTime.Now };
            return await service.EditReminderAsync(_request, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
