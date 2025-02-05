using FluentValidation;
using MReminders.Mobile.Application.Requests.Reminders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MReminders.Mobile.Application.Validators
{
    public class AddReminderRequestValidator : AbstractValidator<AddReminderRequest>
    {
        public AddReminderRequestValidator()
        {
            RuleFor(x => x).NotEmpty().NotNull().WithMessage("O lembrete não pode estar vazio");
            RuleFor(x => x.DueDate).NotEmpty().NotNull().WithMessage("A data não pode estar vazia");
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("O Nome não pode estar vazio");
            RuleFor(x => x.Description).NotEmpty().NotNull().WithMessage("A descrição não pode estar vazia");
        }
    }
}
