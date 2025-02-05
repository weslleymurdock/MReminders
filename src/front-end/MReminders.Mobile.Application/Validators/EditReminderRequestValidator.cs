using FluentValidation;
using MReminders.Mobile.Application.Requests.Reminders;

namespace MReminders.Mobile.Application.Validators;

public class EditReminderRequestValidator : AbstractValidator<EditReminderRequest>
{
    public EditReminderRequestValidator()
    {
        RuleFor(x => x).NotNull().NotEmpty().WithMessage("O lembrete editado não pode estar nulo ou vazio");
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("O Nome (ou Titulo) do lembrete editado não pode estar nulo ou vazio");
        RuleFor(x => x.Description).NotNull().NotEmpty().WithMessage("A descrição do lembrete editado não pode estar nula ou vazia");
    }
}
