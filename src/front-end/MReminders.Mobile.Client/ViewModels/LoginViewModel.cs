using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MReminders.Mobile.Infrastructure.Attributes;

namespace MReminders.Mobile.Client.ViewModels;

[Authorize(false)] 
public partial class LoginViewModel : BaseViewModel
{
    [ObservableProperty]
    private string key = string.Empty;
    
    [ObservableProperty]
    private string password = string.Empty;
    
    public LoginViewModel()
    {
        Title = "Login";
    }

    [RelayCommand]
    private async Task Login()
    {

    }

    [RelayCommand]
    private async Task Register()
    {

    }

    [RelayCommand]
    private async Task TouchId()
    {

    }
}
