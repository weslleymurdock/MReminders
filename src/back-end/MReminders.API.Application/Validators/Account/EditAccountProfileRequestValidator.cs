using FluentValidation;
using MReminders.API.Application.Requests.Account;
using MReminders.API.Infrastructure.Interfaces;
using System.Net.Mail;

namespace MReminders.API.Application.Validators.Account;

public class EditAccountProfileRequestValidator : AbstractValidator<EditAccountProfileRequest>
{
    private readonly IIdentityService identity;
    public EditAccountProfileRequestValidator(IIdentityService identity)
    {
        this.identity = identity;
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The request object must not be null or empty");
        RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("The id must not be null or empty");
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("The Name must not be null or empty");
        RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("The email must not be null or empty");
        RuleFor(x => x.Id).Must(id => Guid.TryParse(id, out _)).WithMessage("The id must be a valid guid string");
        RuleFor(x => x.Id).MustAsync(BelongToAValidExistingUser).WithMessage("The id must be a valid guid string");
        RuleFor(x => x).MustAsync(async (request, token) => await BeUniqueIfNewOrLikeOlderEmail(request.Email, request.Id, token)).WithMessage("The email must be unique");
        RuleFor(x => x.Email).Must(email => MailAddress.TryCreate(email, out _)).WithMessage("The email must have a valid email format");
        RuleFor(x => x.PhoneNumber).NotNull().NotEmpty().WithMessage("The phone number must not be null or empty");
    }

    private async Task<bool> BeUniqueIfNewOrLikeOlderEmail(string validationEmail, string id, CancellationToken token)
    {
        try
        {
            (string userId, string fullName, string UserName, string email, string phone, IList<string> roles) = await identity.GetUserDetails(id);
            if (email == validationEmail)
            {
                return true;
            }
            return await identity.IsUniqueEmail(validationEmail);
        }
        catch 
        {
            return false;
        }
    }

    private async Task<bool> BelongToAValidExistingUser(string id, CancellationToken token) => await identity.GetUserName(id) != string.Empty;

}
