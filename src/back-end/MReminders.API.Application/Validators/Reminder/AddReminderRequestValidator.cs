using FluentValidation;
using MReminders.API.Application.Requests.Reminders;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Validators.Reminder;

public class AddReminderRequestValidator : AbstractValidator<AddReminderRequest>
{
    private readonly IReminderService service;
    private readonly IIdentityService identity;
    public AddReminderRequestValidator(IReminderService service, IIdentityService identity)
    {
        this.service = service;
        this.identity = identity;   
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The request object must not be null or empty").WithErrorCode("400");
        RuleFor(x => x.UserId).Must(HaveGuidPattern).WithMessage("The UserId must be a valid guid string");
        RuleFor(x => x.UserId).MustAsync(BelongToAValidUser).WithMessage("The UserId must be a valid guid string");
        RuleFor(x => x.Name).Must(BeUniqueReminderName).WithMessage("The reminder name should not be repeated");
        RuleFor(x => x.DueDate).Must(NotBeAPastDate).WithMessage("The DueDate of reminder must be in the future");
    }

    private bool NotBeAPastDate(DateTime time) => time >= DateTime.Now;

    private bool BeUniqueReminderName(string name) => !service.GetReminders().Any(x => x.Name == name);

    private async Task<bool> BelongToAValidUser(string id, CancellationToken cancellationToken) => (await identity.GetUserName(id)) != string.Empty;

    private bool HaveGuidPattern(string id) => Guid.TryParse(id, out _);
}
