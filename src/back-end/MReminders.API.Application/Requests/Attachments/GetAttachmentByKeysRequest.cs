using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Attachment;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Domain.Entities;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Requests.Attachments;

public class GetAttachmentByKeysRequest : IRequest<BaseResponse<IEnumerable<AttachmentResponse>>>
{
    public string UserId { get; set; } = string.Empty; 
    public string FileName { get; set; } = string.Empty;
}

public class GetAttachmentByKeysRequestHandler(IReminderService reminderService, IMapper mapper, ILogger<GetAttachmentByKeysRequestHandler> logger) : IRequestHandler<GetAttachmentByKeysRequest, BaseResponse<IEnumerable<AttachmentResponse>>>
{
    public async Task<BaseResponse<IEnumerable<AttachmentResponse>>> Handle(GetAttachmentByKeysRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var attachments = reminderService.GetAttachments().Where(x => x.FileName.Contains(request.FileName, StringComparison.Ordinal) 
            && x.Reminder.UserId == request.UserId);
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

