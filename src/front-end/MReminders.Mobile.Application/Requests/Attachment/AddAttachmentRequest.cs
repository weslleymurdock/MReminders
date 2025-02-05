using MediatR;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Application.Requests.Attachment;

public class AddAttachmentRequest : IRequest<AttachmentResponseBaseResponse>
{
    public byte[] Content { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string ReminderId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}

public class AddAttachmentRequestHandler(IRemindersService service, ILogger<AddAttachmentRequestHandler> logger) : IRequestHandler<AddAttachmentRequest, AttachmentResponseBaseResponse>
{
    public async Task<AttachmentResponseBaseResponse> Handle(AddAttachmentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var _request = new Rest.Client.AddAttachmentRequest()
            {
                Content = request.Content,
                FileName = request.FileName,
                ReminderId = request.ReminderId,
                ContentType = request.ContentType
            };
            return await service.AddAttachmentAsync(_request, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
