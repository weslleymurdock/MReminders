using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Application.Requests.Reminders;
using MReminders.Mobile.Client.Views;
using MReminders.Rest.Client;
using System.Collections.ObjectModel;

namespace MReminders.Mobile.Client.ViewModels;

public partial class ReminderViewModel : AuthorizeViewModel
{
    #region Services
    private readonly ILogger<ReminderViewModel> logger;
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
    public ReminderViewModel()
    {
        logger = Services.GetRequiredService<ILogger<ReminderViewModel>>();
        Title = "Adicionar Lembrete";
    }

    [RelayCommand]
    private async Task AddReminder()
    {
        try
        {
            var dateParsed = DateTime.TryParse(string.Concat(Date.ToString().Split(' ')[0], " ", Time.ToString()), out DateTime DueDate);
            if (!dateParsed) 
            {
                await CurrentPage.DisplayAlert(Title, "Insira uma data valida", "Ok");
                return;
            };
            var userId = await stringStorage.GetAsync(stringStorage.UserId);
            var request = new Application.Requests.Reminders.AddReminderRequest() { Description = Description, DueDate = DueDate, Location = Location, Name = ReminderTitle, UserId = userId };
            var response = await mediator.Send(request);
            if (response.Success)
            {
                await CurrentPage.DisplayAlert(Title, "Lembrete adicionado com sucesso", "Ok");
                await Navigator.PushAsync(new MainPage(), true);
                return;
            }
            else
            {
                await CurrentPage.DisplayAlert(Title, "Lembrete não adicionado, revise os dados e tente novamente", "Ok");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
