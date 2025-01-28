using FluentValidation;
using MReminders.API.Application.Requests.Attachments;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Validators.Attachment;

public class DeleteAttachmentRequestValidator : AbstractValidator<DeleteAttachmentRequest>
{
    private readonly IReminderService service;
    public DeleteAttachmentRequestValidator(IReminderService service)
    {
        this.service = service;
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The request object must not be null or empty");
        RuleFor(x => x.AttachmentId).Must(HaveGuidPattern).WithMessage("The attachment id must be a string with a valid GUID content");
        RuleFor(x => x.AttachmentId).Must(BelongsToAnyAttachment).WithMessage("The attachment id must belong to a valid and existing Attachment");
    }
     
    private bool BelongsToAnyAttachment(string id) => service.GetAttachments().Any(x => x.Id == id);

    private bool HaveGuidPattern(string id) => Guid.TryParse(id, out _);
}
