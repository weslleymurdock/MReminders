using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Attachment;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Infrastructure.Interfaces;
namespace MReminders.API.Application.Requests.Attachments;

public class GetAttachmentByReminderRequest : IRequest<BaseResponse<IEnumerable<AttachmentResponse>>>
{
    public string ReminderId { get; set; } = string.Empty;
}

public class GetAttachmentByReminderRequestHandler(IReminderService reminderService, IMapper mapper, ILogger<GetAttachmentByReminderRequestHandler> logger) : IRequestHandler<GetAttachmentByReminderRequest, BaseResponse<IEnumerable<AttachmentResponse>>>
{
    public async Task<BaseResponse<IEnumerable<AttachmentResponse>>> Handle(GetAttachmentByReminderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var attachments = reminderService.GetAttachments().Where(x => x.ReminderId == request.ReminderId);
            if (attachments is null || !attachments.Any())
            {
                return new BaseResponse<IEnumerable<AttachmentResponse>>() { Data = null!, Message = "Reminder not found", StatusCode = 404, Success = false };
            }
            var response = new BaseResponse<IEnumerable<AttachmentResponse>>() { Data = attachments.Select(x => mapper.Map<AttachmentResponse>(x)), Message = "Ok", StatusCode = 200, Success = true };
            return await Task.FromResult(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new BaseResponse<IEnumerable<AttachmentResponse>>() { Data = [], Message = e.Message, StatusCode = 400, Success = false };
        }
    }
}

