using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Application.Requests.Account;
using MReminders.Rest.Client;

namespace MReminders.Mobile.Client.ViewModels;

public partial class ProfileViewModel : AuthorizeViewModel
{
    #region Services
    private readonly ILogger<ProfileViewModel> logger;
    #endregion

    #region Observable Properties

    [ObservableProperty]
    private string name = string.Empty;
    
    [ObservableProperty]
    private string email = string.Empty;
    
    [ObservableProperty]
    private string phone = string.Empty;
    
    [ObservableProperty]
    private string oldPassword = string.Empty;
    
    [ObservableProperty]
    private string newPassword = string.Empty;
    
    [ObservableProperty]
    private string confirmationNewPassword = string.Empty;

    #endregion
    #region Properties
    public UserResponse User { get; set; } = default!;
    #endregion
    public ProfileViewModel()
    {
        logger = Services.GetRequiredService<ILogger<ProfileViewModel>>();
        GetUser().ConfigureAwait(false);
    }

    private async Task GetUser()
    {
        try
        {
            var key = await stringStorage.GetAsync(stringStorage.UserKey);
            if (string.IsNullOrEmpty(key)) 
            {
                await CurrentPage.DisplayAlert(Title, "Chave de usuário inválida. por favor faça o login novamente com usuário e senha", "Ok");
                await Navigator.PopToRootAsync();
                return;
            }
            User = await mediator.Send(new GetProfileRequest());
            Name = User.Name;
            Email = User.Email;
            Phone = User.PhoneNumber;
            
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private async Task EditProfile()
    {
        try
        {
            EditProfileRequest request = new()
            {
                NewPassword = NewPassword,
                ConfirmationNewPassword = ConfirmationNewPassword,
                Email = Email,
                Name = Name,
                OldPassword = OldPassword,
                Phone = Phone
            };
            var response = await mediator.Send(request);
            if (response.Success)
            {
                await CurrentPage.DisplayAlert(Title, "Usuário atualizado com sucesso", "Ok");
                await Navigator.PopToRootAsync();
            }
            else
            {
                await CurrentPage.DisplayAlert(Title, $"Usuário não atualizado. Verifique os dados e tente novamente! (Erro: {response.Message})", "Ok");
                return;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
