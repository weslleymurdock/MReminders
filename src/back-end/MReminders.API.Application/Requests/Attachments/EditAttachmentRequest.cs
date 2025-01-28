using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Application.Responses.Attachment;
using MReminders.API.Domain.Entities;
using MReminders.API.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MReminders.API.Application.Requests.Attachments;

public class EditAttachmentRequest : IRequest<BaseResponse<AttachmentResponse>>
{
    public string AttachmentId { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string ReminderId { get; set; } = string.Empty;
}


public class EditAttachmentRequestHandler(IReminderService service, IMapper mapper, IValidator<EditAttachmentRequest> validator, ILogger<EditAttachmentRequestHandler> logger) : IRequestHandler<EditAttachmentRequest, BaseResponse<AttachmentResponse>>
{
    public async Task<BaseResponse<AttachmentResponse>> Handle(EditAttachmentRequest request, CancellationToken cancellationToken)
    {
		try
		{
			var result = await validator.ValidateAsync(request, cancellationToken);

			if (!result.IsValid)
			{
				return new() { Data = default!, Message = string.Join("\n ", result.Errors), Success = false, StatusCode = 422 };
			}

            var attachment = mapper.Map<Attachment>(request);

			var _attachment = service.GetAttachment(x => x.Id == request.AttachmentId) ?? throw new KeyNotFoundException("Attachment not found");

			_attachment.Id = request.AttachmentId;
			_attachment.Content = request.Content;
			_attachment.ContentType = request.ContentType;
			_attachment.ModifiedDate = DateTime.Now;
			_attachment.FileName = request.FileName;
			_attachment.ReminderId = request.ReminderId;
			
			var response = await service.EditAttachment(_attachment);

			if (!response)
			{
                return new() { Data = default!, Message = "Error when editing attachment", Success = response, StatusCode = 409 };
            }

            return new() { Data = mapper.Map<AttachmentResponse>(attachment), Message = "Ok", Success = true, StatusCode = 200 };
		}
		catch (Exception e)
		{
			logger.LogError(e, e.Message);
			return new BaseResponse<AttachmentResponse>() { Data = { }, Message = e.Message, StatusCode = 400, Success = false };
        }
    }
}
