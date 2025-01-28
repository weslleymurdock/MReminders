using MReminders.Mobile.Infrastructure.Attributes;

namespace MReminders.Mobile.Client.ViewModels;

[Authorize(false)]
public partial class RegisterViewModel : BaseViewModel
{
    public RegisterViewModel()
    {
        Title = "Cadastro";
    }
}
