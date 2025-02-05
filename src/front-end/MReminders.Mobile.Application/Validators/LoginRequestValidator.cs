using FluentValidation;
using MReminders.Mobile.Application.Requests.Account;
using System.Text.RegularExpressions;

namespace MReminders.Mobile.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("O objeto da requisiçao nao pode ser nulo ou vazio.").WithErrorCode("400");
        RuleFor(x => x.Key).NotNull().NotEmpty().WithMessage("A propriedade Key da requisiçao nao pode ser nulo ou vazio.").WithErrorCode("400");
        RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("A propriedade Password da requisiçao nao pode ser nulo ou vazio.").WithErrorCode("400");
        RuleFor(x => x.Key).MustAsync(BeAUserNameEmailOrPhoneNumber).WithMessage("A propriedade Key deve ser uma string com o formato de email, numero de telefone ou nome de usuário").WithErrorCode("422") ;
        RuleFor(x => x.Password).Must(HavePasswordPattern).WithMessage("A propriedade Password deve ser uma string contendo no minimo 8 caracteres, 1 letra maiuscula, 1 letra minuscula, 1 numero e 1 caractere especial").WithErrorCode("422") ;
    }

    private Task<bool> BeAUserNameEmailOrPhoneNumber(string key, CancellationToken token) => Task.FromResult(new Regex("^[a-zA-Z0-9]{3,15}$").IsMatch(key) || System.Net.Mail.MailAddress.TryCreate(key, out _) || new Regex(@"^\+[0-9]{10,13}$").IsMatch(key));
    private bool HavePasswordPattern(string password) => new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$").IsMatch(password);
}
