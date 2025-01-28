using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Application.Responses.Reminders;
using MReminders.API.Domain.Entities;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Requests.Reminders;

public class AddReminderRequest : IRequest<BaseResponse<ReminderResponse>>
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime DueDate { get; set; } 

}

public class AddReminderRequestHandler(IReminderService service, IValidator<AddReminderRequest> validator, IMapper mapper, ILogger<AddReminderRequestHandler> logger) : IRequestHandler<AddReminderRequest, BaseResponse<ReminderResponse>>
{
    public async Task<BaseResponse<ReminderResponse>> Handle(AddReminderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            
            if (!result.IsValid)
            {
                return new() { Data = null!, Message = string.Join("\n ", result.Errors), Success = false, StatusCode = 422 };
            }

            var reminder = mapper.Map<Reminder>(request);

            var response = await service.AddReminder(reminder);

            return new() { Data = mapper.Map<ReminderResponse>(response), Message = response is null? "Not Ok" : "Ok", Success = response is not null, StatusCode = response is null ? 409 : 201 };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new() { Data = null!, Message = $"There was an error when adding reminder: {e.Message}", Success = false, StatusCode = 400 };
        }
    }
}
