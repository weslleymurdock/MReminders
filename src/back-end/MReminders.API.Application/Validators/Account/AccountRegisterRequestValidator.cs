using FluentValidation;
using FluentValidation.Validators;
using MReminders.API.Application.Requests.Account;
using MReminders.API.Infrastructure.Interfaces;
using System.Net.Mail;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace MReminders.API.Application.Validators.Account;

public class AccountRegisterRequestValidator : AbstractValidator<RegisterAccountRequest>
{
    private readonly IIdentityService service;
    public AccountRegisterRequestValidator(IIdentityService service)
    {
        this.service = service;
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The username object must not be null or empty");
        RuleFor(x => x.UserName).NotNull().NotEmpty().WithMessage("The username must be informed");
        RuleFor(x => x.UserName).Must(username => new Regex(@"^[a-zA-Z]{6,}$").IsMatch(username)).WithMessage("The username must not have special chars or numbers and must have at least 6 chars length");
        RuleFor(x => x.UserName).MustAsync(BeUniqueUsername).WithMessage("This username already exists");
        RuleFor(x => x.Password).Must(HavePasswordPattern).WithMessage("The password must have a lowercase char, a uppercase char, a special character and have above than 8 digits length");
        RuleFor(x => x.ConfirmationPassword).Must(HavePasswordPattern).WithMessage("The confirmation password must have a lowercase char, a uppercase char, a special character and have above than 8 digits length");
        RuleFor(x => x).Must(request => (request.Password == request.ConfirmationPassword)).WithMessage("The password and confirmation password must match");
        RuleFor(x => x.PhoneNumber).Must(HavePhoneNumberPattern).WithMessage("The phone number must be a valid phone number");
        RuleFor(x => x.PhoneNumber).MustAsync(BeUniquePhoneNumber).WithMessage("The phone number is already in use");
        RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("The email must be informed");
        RuleFor(x => x.Email).Must(HaveEmailPattern).WithMessage("The email must be a valid email");
        RuleFor(x => x.Email).MustAsync(BeUniqueEmail).WithMessage("The email is already in use");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken) => await service.IsUniqueEmail(email);
    private async Task<bool> BeUniquePhoneNumber(string phoneNumber, CancellationToken token) => await service.IsUniquePhoneNumber(phoneNumber);
    private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken) => (await service.IsUniqueUserName(username));
    private bool HaveEmailPattern(string email) => MailAddress.TryCreate(email, out _);
    private bool HavePasswordPattern(string password) => new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$").IsMatch(password);
    private bool HavePhoneNumberPattern(string phoneNumber) => new Regex(@"\+[1-9]{1}[0-9]{1,14}$").IsMatch(phoneNumber);

}
