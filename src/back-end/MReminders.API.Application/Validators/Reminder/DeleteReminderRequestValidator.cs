using FluentValidation;
using MReminders.API.Application.Requests.Reminders;

namespace MReminders.API.Application.Validators.Reminder;
/// <summary>
/// 
/// </summary>
public class DeleteReminderRequestValidator : AbstractValidator<DeleteReminderRequest>
{
    public DeleteReminderRequestValidator()
    {
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The request object must not be null or empty");
        RuleFor(x => x.ReminderId).NotNull().NotEmpty().WithMessage("The ReminderId field must not be null or empty");
        RuleFor(x => x.ReminderId).Must(NotBeWhiteSpace).WithMessage("The ReminderId field must not be null or white space").WithErrorCode("Vam");
        RuleFor(x => x.ReminderId).Must(HaveGuidPattern).WithMessage("The ReminderId field must have be a valid guid").WithErrorCode("422") ;

    }

    /// <summary>
    /// Validates if the <paramref name="id"/>  of reminder is a valid Guid string
    /// </summary>
    /// <param name="id">the id from the <see cref="DeleteReminderRequest"/> object</param>
    /// <returns>true if <paramref name="id"/> is a valid Guid string, otherwise false</returns>
    private bool HaveGuidPattern(string id) => Guid.TryParse(id, out _);

    /// <summary>
    /// Validates if the <paramref name="id"/> of reminder have is null or whiteSpace
    /// </summary>
    /// <param name="id">the <paramref name="id"/>  from the <see cref="DeleteReminderRequest"/> object</param>
    /// <returns>true if the <paramref name="id"/> isnt white space or a null string</returns>
    private bool NotBeWhiteSpace(string id) => !string.IsNullOrWhiteSpace(id);
    
}
