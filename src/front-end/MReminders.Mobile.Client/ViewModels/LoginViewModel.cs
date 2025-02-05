using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Application.Requests.Account;
using MReminders.Mobile.Client.Views;
using MReminders.Mobile.Infrastructure.Attributes;

namespace MReminders.Mobile.Client.ViewModels;

[Authorize(false)]
public partial class LoginViewModel : BaseViewModel
{
    #region injections
    private readonly ILogger<LoginViewModel> logger;
    #endregion
    [ObservableProperty]
    private string key = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public LoginViewModel()
    {
        logger = Services.GetRequiredService<ILogger<LoginViewModel>>();
        Title = "Login";
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            var result = await mediator.Send(new LoginRequest() { Key = Key, Password = Password });
            if (result.Success)
            {
                await this.Navigator.PushAsync(new MainPage(), true);
            }
            else
            {
                await CurrentPage.DisplayAlert(Title, "Usuário ou senha inválidos", "Ok");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            await CurrentPage.DisplayAlert(Title, "Erro: " + e.Message, "Ok");
        }
    }

    [RelayCommand]
    private async Task Register()
    {
        try
        {
            await this.Navigator.PushAsync(new RegisterPage(), true);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            await CurrentPage.DisplayAlert(Title, "Erro: " + e.Message, "Ok");
        }
    }

    [RelayCommand]
    private async Task BiometricLogin()
    {
        try
        {
            BiometricLoginRequest request = new()
            {
                BasicToken = (await tokenStorage.GetTokenAsync(Domain.Enums.TokenKind.Basic)).Value
            };
            var result = await mediator.Send(request);
            if (result.Success)
            {
                await Navigator.PushAsync(new MainPage(), true);
            }
            else
            {
                await CurrentPage.DisplayAlert(Title, "Autenticação com falha. Tente novamente", "Ok");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            await CurrentPage.DisplayAlert(Title, "Erro: Biometria indisponível no momento", "Ok");
        }
    }

    [RelayCommand]
    private async Task PasswordEntryFocused()
    {

    }
}
