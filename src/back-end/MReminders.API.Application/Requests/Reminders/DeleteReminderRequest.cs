using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Requests.Reminders;

public class DeleteReminderRequest : IRequest<BaseResponse<bool>>
{
    public string ReminderId { get; set; } = string.Empty;
}

public class DeleteReminderRequestHandler(IReminderService service, ILogger<DeleteReminderRequestHandler> logger, IValidator<DeleteReminderRequest> validator) : IRequestHandler<DeleteReminderRequest, BaseResponse<bool>>
{
    public async Task<BaseResponse<bool>> Handle(DeleteReminderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
            {
                return new BaseResponse<bool> { Data = false, Message = string.Join("\n ", result.Errors), Success = false, StatusCode = 422 };
            }
            var reminder = service.GetReminders().Where(x => x.Id == request.ReminderId).FirstOrDefault() ?? new Domain.Entities.Reminder() { Id = "" };
            if (reminder.Id == string.Empty)
            {
                return new BaseResponse<bool>()
                {
                    Data = false,
                    Message = "Reminder not found",
                    StatusCode = 404,
                    Success = false
                };
            }
            var response = await service.DeleteReminder(reminder);
            return new BaseResponse<bool>() { Data = response, Message = response ? "Successfultly deleted" : "Not deleted due to errors", Success = response, StatusCode = response? 200 : 409 };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new BaseResponse<bool>()
            {
                Data = false,
                Message = $"Error when deleting reminder: {e.Message}",
                StatusCode = 400,
                Success = false
            };
        }
    }
}
