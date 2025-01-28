using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Infrastructure.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MReminders.API.Application.Requests.Attachments;

public class DeleteAttachmentRequest : IRequest<BaseResponse<bool>>
{
    public string AttachmentId { get; set; } = string.Empty;
}

public class DeleteAttachmentRequestHandler(IReminderService service, ILogger<DeleteAttachmentRequestHandler> logger, IValidator<DeleteAttachmentRequest> validator) : IRequestHandler<DeleteAttachmentRequest, BaseResponse<bool>>
{
    public async Task<BaseResponse<bool>> Handle(DeleteAttachmentRequest request, CancellationToken cancellationToken)
    {
		try
		{
			logger.LogInformation($"{nameof(DeleteAttachmentRequestHandler)} starts");

            var result = await validator.ValidateAsync(request, cancellationToken);
			if (!result.IsValid) 
			{
				return new BaseResponse<bool> { Data = false, Message = string.Join("\n ", result.Errors), Success = false, StatusCode = 422 };
			}	
			var attachment = service.GetAttachment(x => x.Id == request.AttachmentId)?? throw new KeyNotFoundException("Attachment not found");
			var response = await service.DeleteAttachment(attachment);
			return new BaseResponse<bool>() { Data = response, Message = response ? "Successfully deleted" : "Not deleted", Success = response , StatusCode = response ? 200 : 409 };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            return new BaseResponse<bool>() { Data = false, Message = e.Message, StatusCode = 400, Success = false };
        }
        finally	
		{ 
            logger.LogInformation($"{nameof(DeleteAttachmentRequestHandler)} finishes");
        }
    }
}
