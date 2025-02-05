using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MReminders.Mobile.Client.Views;
using MReminders.Rest.Client;
using System.Collections.ObjectModel;

namespace MReminders.Mobile.Client.ViewModels;

public partial class EditReminderViewModel : AuthorizeViewModel
{
    #region Services
    private readonly ILogger<EditReminderViewModel> logger;
    #endregion

    #region Properties
    public ReminderResponse Reminder { get; set; } = default!;
    #endregion

    #region Observable properties
#pragma warning disable 
    [ObservableProperty]
    private DateTime date;

    [ObservableProperty]
    private TimeSpan time;

    [ObservableProperty]
    private string reminderTitle = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string location = string.Empty;
#pragma warning restore 
    #endregion
    public EditReminderViewModel()
    {
        logger = Services.GetRequiredService<ILogger<EditReminderViewModel>>();
        Title = "Editar Lembrete";
        SetReminderCommand.Execute(this);
    }

    private async Task SetReminderData()
    {
        var id = await stringStorage.GetAsync(stringStorage.ReminderId);
        Reminder = Reminders.FirstOrDefault(x => x.ReminderId == id)!;
        ReminderTitle = Reminder.ReminderName;
        Description = Reminder.ReminderDescription;
        Location = Reminder.ReminderLocation;
        Date = Reminder.ReminderDate.Date;
        Time = Reminder.ReminderDate.TimeOfDay;
    }

    [RelayCommand]
    private async Task EditReminder()
    {
        try
        {
            var userId = await stringStorage.GetAsync(stringStorage.UserId);
            var reminderId = await stringStorage.GetAsync(stringStorage.ReminderId);
            var dateParsed = DateTime.TryParse(string.Concat(Date.ToString().Split(' ')[0], " ", Time.ToString()), out DateTime DueDate);
            var request = new Application.Requests.Reminders.EditReminderRequest() { Description = Description, DueDate = dateParsed ? DueDate : Reminder.ReminderDate, Location = Location, Name = ReminderTitle, UserId = userId, ReminderId = reminderId };
            var response = await mediator.Send(request);
            if (response.Success)
            {
                await CurrentPage.DisplayAlert(Title, "Lembrete atualizado com sucesso", "Ok");
                await Navigator.PushAsync(new MainPage(), true); 
            }
            else
            {
                await CurrentPage.DisplayAlert(Title, "Lembrete não atualizado, revise os dados e tente novamente", "Ok");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }


    [RelayCommand]
    private async Task SetReminder()
    {
        try
        { 
            await LoadReminders();
            await SetReminderData();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }
}
