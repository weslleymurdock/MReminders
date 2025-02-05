using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Reminders;

public class AddReminderRequest : IRequest<ReminderResponseBaseResponse>
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }

}

public class AddReminderRequestHandler(IRemindersService service, IValidator<AddReminderRequest> validator, ILogger<AddReminderRequestHandler> logger) : IRequestHandler<AddReminderRequest, ReminderResponseBaseResponse>
{
    public async Task<ReminderResponseBaseResponse> Handle(AddReminderRequest request, CancellationToken cancellationToken)
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
            var response = await service.AddReminderAsync(new Rest.Client.AddReminderRequest() { Description = request.Description, DueDate = request.DueDate, Location = request.Location, Name = request.Name, UserId = request.UserId }, cancellationToken);
            return response;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
