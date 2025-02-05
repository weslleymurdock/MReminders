using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Application.Requests.Reminders;
using MReminders.Mobile.Client.Views;
using MReminders.Rest.Client;
using System.Collections.ObjectModel;

namespace MReminders.Mobile.Client.ViewModels;

public partial class AttachmentViewModel : AuthorizeViewModel
{
    #region Services
    private readonly ILogger<AttachmentViewModel> logger;
    #endregion

    #region Observable properties
    
    [ObservableProperty]
    private DateTime date = DateTime.Now;

    [ObservableProperty]
    private TimeSpan time;

    [ObservableProperty]
    private string reminderTitle;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private string location;

    #endregion
    public AttachmentViewModel()
    {
        logger = Services.GetRequiredService<ILogger<AttachmentViewModel>>();
        Title = "Adicionar Lembrete";
    }

    [RelayCommand]
    private async Task AddReminder()
    {
        try
        {
            var userId = await stringStorage.GetAsync(stringStorage.UserId);
            var response = await mediator.Send(new Application.Requests.Reminders.AddReminderRequest() { Description = Description, DueDate = DateTime.Parse(string.Concat(Date.ToString().Split(' ')[0], " ", Time.ToString())), Location = Location, Name = Title, UserId = userId });
            if (response.Success)
            {
                await CurrentPage.DisplayAlert("Sucesso", "Lembrete adicionado com sucesso", "Ok");
                await Navigator.PushAsync(new MainPage(), true);
                return;
            }
            else
            {
                await CurrentPage.DisplayAlert("Erro", "Lembrete não adicionado, revise os dados e tente novamente", "Ok");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
