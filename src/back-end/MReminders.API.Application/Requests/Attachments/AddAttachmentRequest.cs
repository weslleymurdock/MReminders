using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.API.Application.Requests.Account;
using MReminders.API.Application.Responses.Attachment;
using MReminders.API.Application.Responses.Base;
using MReminders.API.Domain.Entities;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Requests.Attachments;

public class AddAttachmentRequest : IRequest<BaseResponse<AttachmentResponse>>
{
    public byte[] Content { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string ReminderId { get; set; } = string.Empty;
}

public class AddAttachmentRequestHandler(IReminderService service, IValidator<AddAttachmentRequest> validator, IMapper mapper, ILogger<AddAttachmentRequestHandler> logger) : IRequestHandler<AddAttachmentRequest, BaseResponse<AttachmentResponse>>
{
    public async Task<BaseResponse<AttachmentResponse>> Handle(AddAttachmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation($"{nameof(AddAttachmentRequestHandler)} started");

            var result = await validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
            {
                return new() { Data = null!, Message = string.Join("\n ", result.Errors), Success = false, StatusCode = 422 };
            }
            var attachment = mapper.Map<Attachment>(request);
            var response = await service.AddAttachment(attachment);
            if (response == null)
            {
                return new BaseResponse<AttachmentResponse> { Data = null!, Message = "Error when processing attachment", Success = false, StatusCode = 409 };
            }
            return new BaseResponse<AttachmentResponse>()
            {
                Data = mapper.Map<AttachmentResponse>(response),
                Message = "Ok",
                StatusCode = 201,
                Success = true
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(AddAttachmentRequestHandler)} finishes");
        }
    }
}

