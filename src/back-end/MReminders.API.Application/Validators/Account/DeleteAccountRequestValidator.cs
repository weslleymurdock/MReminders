using FluentValidation;
using MReminders.API.Application.Requests.Account;
using MReminders.API.Infrastructure.Interfaces;

namespace MReminders.API.Application.Validators.Account;

public class DeleteAccountRequestValidator : AbstractValidator<DeleteAccountRequest>
{
    private readonly IIdentityService identity;
    public DeleteAccountRequestValidator(IIdentityService identity)
    {
        this.identity = identity;
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("The Request Object must not be null or empty");
        RuleFor(x => x.Key).NotNull().NotEmpty().WithMessage("The User Key must not be null or empty");
        RuleFor(x => x).MustAsync(async (request, token) => await BelongToValidUser(request.Key, token)).WithMessage("The key must belong to a valid user");
    }

    private async Task<bool> BelongToValidUser(string key, CancellationToken token) => await identity.GetUserId(key) != string.Empty;
}
