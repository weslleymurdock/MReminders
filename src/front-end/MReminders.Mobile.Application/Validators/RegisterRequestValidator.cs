using FluentValidation;
using MReminders.Mobile.Application.Requests.Account;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace MReminders.Mobile.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x).NotEmpty().NotNull().WithMessage("O cadastro do usuario nao pode estar nulo ou vazio");
        RuleFor(x => x.UserName).NotEmpty().NotNull().WithMessage("O nome de usuario nao pode estar nulo ou vazio");
        RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("O Email nao pode estar nulo ou vazio");
        RuleFor(x => x.Email).Must(BeValidEmail).WithMessage("O email deve ser um email válido");
        RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("O nome não pode estar vazio ou nulo");
        RuleFor(x => x.PhoneNumber).NotNull().NotEmpty().WithMessage("O telefone não pode estar vazio ou nulo");
        RuleFor(x => x.PhoneNumber).Must(HavePhoneNumberPattern).WithMessage("O telefone deve ser um telefone válido");
        RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("A senha não pode estar vazia ou nula");
        RuleFor(x => x.Password).Must(HavePasswordPattern).WithMessage("A senha deve conter no minimo 8 digitos sendo necessário conter 1 caractere especial, 1 numero, 1 caractere minusculo e 1 caractere maiusculo");
        RuleFor(x => x.ConfirmationPassword).NotNull().NotEmpty().WithMessage("A confirmação da senha não pode estar vazia ou nula");
        RuleFor(x => x).Must((x) => BeCorrespondingToPassword(x.Password, x.ConfirmationPassword)).WithMessage("A confirmação da senha deve corresponder à senha informada");
    }

    private bool BeCorrespondingToPassword(string password, string confirmationPassword) => password == confirmationPassword;

    private bool HavePasswordPattern(string password) => new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$").IsMatch(password);

    private bool HavePhoneNumberPattern(string phoneNumber)=> new Regex(@"\+[1-9]{1}[0-9]{1,14}$").IsMatch(phoneNumber);

    private bool BeValidEmail(string email) => MailAddress.TryCreate(email, out _);
}
