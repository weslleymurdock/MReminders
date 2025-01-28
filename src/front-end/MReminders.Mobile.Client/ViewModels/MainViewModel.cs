using MReminders.Mobile.Infrastructure.Attributes;

namespace MReminders.Mobile.Client.ViewModels;

[Authorize(true)]
public partial class MainViewModel : BaseViewModel
{
    public MainViewModel()
    {
        Title = "Lembretes";
    }
}
