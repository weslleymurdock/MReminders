using FluentValidation;
using MReminders.API.Application.Requests.Account;
using System.Text.RegularExpressions;

namespace MReminders.API.Application.Validators.Account;

public class AccountLoginRequestValidator : AbstractValidator<LoginAccountRequest>
{
    public AccountLoginRequestValidator()
    {
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The request object must not be null or empty");
        RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("The password must not be null or empty");
        RuleFor(x => x.Password).Must(HavePasswordPattern).WithMessage("The password must have a lowercase char, a uppercase char, a special character and have above than 8 digits length");
        RuleFor(x => x.Key).NotNull().NotEmpty().WithMessage("The key of account must be informed and a valid username or email");
    }

    private bool HavePasswordPattern(string password) => new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$").IsMatch(password);
}
