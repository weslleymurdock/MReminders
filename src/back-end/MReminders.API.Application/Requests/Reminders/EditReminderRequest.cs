using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Application.Responses.Reminders;
using MReminders.API.Domain.Entities;
using MReminders.API.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MReminders.API.Application.Requests.Reminders;

public class EditReminderRequest : IRequest<BaseResponse<ReminderResponse>>
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime DueDate { get; set; } 
    public bool Done { get; set; }
}


public class EditReminderRequestHandler(IReminderService service, IMapper mapper, IValidator<EditReminderRequest> validator, ILogger<EditReminderRequestHandler> logger) : IRequestHandler<EditReminderRequest, BaseResponse<ReminderResponse>>
{
    public async Task<BaseResponse<ReminderResponse>> Handle(EditReminderRequest request, CancellationToken cancellationToken)
    {
		try
		{
			var validation = await validator.ValidateAsync(request, cancellationToken);

			if (!validation.IsValid)
			{
				return new() { Data = default!, Message = string.Join("\n ", validation.Errors), Success = false, StatusCode = 422 };
			}

            var reminder = mapper.Map<Reminder>(request);

			var _reminder = service.GetReminder(x => x.Id == reminder.Id);

			if (_reminder == null)
			{
				return new BaseResponse<ReminderResponse> { Data = default!, Message = "Reminder not found", StatusCode = 404, Success = false };
			}

			var result = await service.EditReminder(reminder);

			if (!result)
			{
                return new() { Data = default!, Message = "Error when editing reminder", Success = result, StatusCode = 400 };
            }

            return new() { Data = mapper.Map<ReminderResponse>(reminder), Message = "Ok", Success = true, StatusCode = 200 };
		}
		catch (Exception e)
		{
			logger.LogError(e, e.Message);
			throw;
		}
    }
}
