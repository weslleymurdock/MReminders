using FluentValidation;
using MReminders.API.Application.Requests.Attachments;
using MReminders.API.Infrastructure.Interfaces;
using System.Text.RegularExpressions;

namespace MReminders.API.Application.Validators.Attachment;

public class AddAttachmentRequestValidator : AbstractValidator<AddAttachmentRequest>
{
    private readonly IReminderService reminderService;
    public AddAttachmentRequestValidator(IReminderService reminderService)
    {
        this.reminderService = reminderService;
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The request object must not be null or empty").WithErrorCode("400"); 
        RuleFor(x => x.ReminderId).NotNull().NotEmpty().WithMessage("The ReminderId of request object must not be null or empty").WithErrorCode("400");
        RuleFor(x => x.ReminderId).Must(HaveGuidPattern).WithMessage("The ReminderId must have a valid guid pattern").WithErrorCode("422");
        RuleFor(x => x.ReminderId).Must(BelongToAnyReminder).WithMessage("The ReminderId must belong to a valid existent reminder").WithErrorCode("404");
        RuleFor(x => x.FileName).NotNull().NotEmpty().WithMessage("The FileName must not be null or empty").WithErrorCode("400"); 
        RuleFor(x => x).Must(request => NotHaveDuplicatedFileName(request.FileName, request.ReminderId)).WithMessage("The filename must be unique among the attachments file names").WithErrorCode("409");
        RuleFor(x => x.Content).NotNull().NotEmpty().WithMessage("The file Content must not be null or empty").WithErrorCode("422");
        RuleFor(x => x.ContentType).NotNull().NotEmpty().WithMessage("The file ContentType must not be null or empty").WithErrorCode("400");
    }
     
    private bool NotHaveDuplicatedFileName(string filename, string reminderId) => !reminderService.GetReminder((Func<Domain.Entities.Reminder, bool>)(x => x != null)).Attachments.Any(x => x.FileName == filename && x.ReminderId == reminderId); 

    private bool BelongToAnyReminder(string reminderId) => reminderService.GetReminders().Any(x => x.Id == reminderId);

    private bool HaveGuidPattern(string reminderId) => Guid.TryParse(reminderId, out _);
}
