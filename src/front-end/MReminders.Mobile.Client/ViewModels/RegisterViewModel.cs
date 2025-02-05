using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Application.Requests.Account;
using MReminders.Mobile.Client.Views;
using MReminders.Mobile.Infrastructure.Attributes;

namespace MReminders.Mobile.Client.ViewModels;

[Authorize(false)]
public partial class RegisterViewModel : BaseViewModel
{
    #region Services
    private ILogger<RegisterViewModel> logger;
    #endregion
    #region Observable Properties
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmationPassword = string.Empty;

    #endregion
    public RegisterViewModel()
    {
        logger = Services.GetRequiredService<ILogger<RegisterViewModel>>();
        Title = "Cadastro";
    }
    [RelayCommand]
    private async Task Register()
    {
        try
        {
            logger.LogInformation($"{nameof(Register)} starts");
            RegisterRequest request = new ()
            {
                ConfirmationPassword = ConfirmationPassword,
                Password = Password,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Name = Name,
                UserName = Username,
                Roles = ["user"]
            };
            var response = await mediator.Send(request);
            if (!response.Success)
            {
                await CurrentPage.DisplayAlert(Title, response.Message, "Ok");
                return;
            }
            await Navigator.PushAsync(new LoginPage(), true);

        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            logger.LogInformation($"{nameof(Register)} ends");
        }
    }
}
