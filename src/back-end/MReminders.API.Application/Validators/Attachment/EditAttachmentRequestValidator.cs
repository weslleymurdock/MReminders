using FluentValidation;
using MReminders.API.Application.Requests.Attachments;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Validators.Attachment;

public class EditAttachmentRequestValidator : AbstractValidator<EditAttachmentRequest>
{
    private readonly IReminderService service;
    public EditAttachmentRequestValidator(IReminderService service)
    {
        this.service = service;
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The request object must not be null or empty").WithErrorCode("400");
        RuleFor(x => x.AttachmentId).NotNull().NotEmpty().WithMessage("The AttachmentId must not be null or empty").WithErrorCode("422");
        RuleFor(x => x.ReminderId).NotNull().NotEmpty().WithMessage("The ReminderId must not be null or empty").WithErrorCode("422");
        RuleFor(x => x.AttachmentId).Must(HaveGuidPattern).WithMessage("The AttachmentId must be a valid Guid string").WithErrorCode("422");
        RuleFor(x => x.ReminderId).Must(HaveGuidPattern).WithMessage("The ReminderId must be a valid Guid string").WithErrorCode("422");
        RuleFor(x => x.AttachmentId).Must(BelongToAValidAttachment).WithMessage("The AttachmentId must belong to a valid existent attachment").WithErrorCode("404");
        RuleFor(x => x.ReminderId).Must(BelongToAValidReminder).WithMessage("The ReminderId must belong to a valid existent reminder").WithErrorCode("404");
        RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage("The File Content must not be null or empty").WithErrorCode("400");
        RuleFor(x => x.ContentType).NotNull().NotEmpty().WithMessage("The File ContentType must not be null or empty").WithErrorCode("400");
        RuleFor(x => x.FileName).NotNull().NotEmpty().WithMessage("The File Name must not be null or empty").WithErrorCode("400");
        RuleFor(x => x).Must(request => NotHaveDuplicatedFileName(request.FileName, request.ReminderId)).WithMessage("The filename must be unique among the attachments file names").WithErrorCode("409");
    }

    private bool NotHaveDuplicatedFileName(string fileName, string reminderId) => !service.GetAttachments().Any(x => x.FileName == fileName && x.ReminderId == reminderId);

    private bool HaveGuidPattern(string id) => Guid.TryParse(id, out _);

    private bool BelongToAValidReminder(string reminderId) => service.GetReminders().Any(x => x.Id == reminderId);

    private bool BelongToAValidAttachment(string attachmentId) => service.GetAttachments().Any(x => x.Id == attachmentId);
}
