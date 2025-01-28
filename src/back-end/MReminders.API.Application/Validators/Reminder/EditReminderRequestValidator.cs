using FluentValidation;
using MReminders.API.Application.Requests.Reminders;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Validators.Reminder;

public class EditReminderRequestValidator : AbstractValidator<EditReminderRequest>
{
    private readonly IReminderService service;
    private readonly IIdentityService identity;
    public EditReminderRequestValidator(IReminderService service, IIdentityService identity)
    {
        this.service = service;
        this.identity = identity;
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The request object must not be null or empty");
        RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("The Id must informed");
        RuleFor(x => x.Id).Must(BelongToValidReminder).WithMessage("The Id must belong to a valid existent reminder");
        RuleFor(x => x).Must(request => BeUniqueReminderName(request.Name, request.UserId)).WithMessage("The reminder name should not be repeated");
        RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("The UserId must be informed");
        RuleFor(x => x.UserId).MustAsync(BelongToAValidUser).WithMessage("The UserId must be a valid guid string");
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("The name must not be null or empty");
        RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("The description must not be null or empty");
        RuleFor(x => x.DueDate).Must(NotBeAPastDate).WithMessage("The DueDate of reminder must be in the future");

    }
    private bool NotBeAPastDate(DateTime time) => time >= DateTime.Now;

    private async Task<bool> BelongToAValidUser(string userId, CancellationToken token) => (await identity.GetUserName(userId) != string.Empty);

    private bool BelongToValidReminder(string id) => service.GetReminders().Any(x => x.Id == id);

    private bool BeUniqueReminderName(string name, string userId) => !service.GetReminders().Any(x => x.Name == name && x.UserId == userId);
}
