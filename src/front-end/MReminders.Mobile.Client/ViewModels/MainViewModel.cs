using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HeyRed.Mime;
using Microsoft.Extensions.Logging;
using MReminders.Mobile.Client.Messages;
using MReminders.Mobile.Client.Views;
using MReminders.Rest.Client;
using System.Collections.ObjectModel;
using System.IO;
using MReminders.Mobile.Infrastructure.Services;

using System.Net.Mime;

#if IOS
using UIKit;
using UniformTypeIdentifiers;
using Foundation;
#endif
namespace MReminders.Mobile.Client.ViewModels;

public partial class MainViewModel : AuthorizeViewModel
{
    #region Services
    private readonly ILogger<MainViewModel> logger;
    #endregion

    #region Observable Properties

    [ObservableProperty]
    private string reminderFilter = string.Empty;

    [ObservableProperty]
    private bool isRefreshing = false;

    [ObservableProperty]
    private DateTime reminderDateFilter = DateTime.Today;

    [ObservableProperty]
    private DateTime minimumDateFilter = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<ReminderResponse> filteredReminders = [];
    #endregion

    #region Fields
    public string filePath = string.Empty;
    #endregion
    public MainViewModel()
    {
        logger = Services.GetRequiredService<ILogger<MainViewModel>>();
        Title = "Lembretes";
        RefreshRemindersCommand.Execute(this);
        WeakReferenceMessenger.Default.Register<FilePathMessage>(this, async (sender, message) =>
        {
            await OnFilePicked(message);
        });
    }

    #region Message handlers

    private async Task OnFilePicked(FilePathMessage message)
    {
        try
        {
            var reminderId = await stringStorage.GetAsync(stringStorage.ReminderId);
            await AddAttachment(message.Value, reminderId);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    #endregion


    #region Methods
    private async Task AddAttachment(string filePath, string reminderId)
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] fileBuffer = memoryStream.ToArray();
            string contentType = MimeTypesMap.GetMimeType(filePath);
            var response = await mediator.Send(new Application.Requests.Attachment.AddAttachmentRequest() { Content = fileBuffer, ContentType = contentType, FileName = filePath, ReminderId = reminderId });
            if (response.Success)
            {
                await CurrentPage.DisplayAlert(Title, "Anexo adicionado ao lembrete", "Ok");
            }
            else
            {
                await CurrentPage.DisplayAlert(Title, "Anexo adicionado ao lembrete", "Ok");
            }
        }
        catch (Exception e)
        {

            throw;
        }
    }

#if IOS
    private string GetMimeType(string filePath)
    {
        var fileExtension = Path.GetExtension(filePath);
        if (!string.IsNullOrEmpty(fileExtension))
        {
#if IOS14_0_OR_GREATER
            var mimeType = UTType.CreateFromExtension(fileExtension);
            return mimeType.ToString();
#endif
        }
        return "application/octet-stream";
    }
#endif
    #endregion

    #region Commands
    [RelayCommand]

    private async Task AddReminderAttachment(ReminderResponse reminder)
    {
        try
        {
#if IOS
            var documentPickerService = new DocumentPickerService();
            var pickedFile = await documentPickerService.PickFileAsync();
              
#elif ANDROID
            await Task.Run(() => { var intent = new Android.Content.Intent(Android.Content.Intent.ActionGetContent);
            intent.SetType("*/*");
            Platform.CurrentActivity!.StartActivityForResult(intent, 0);});
#endif
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private async Task AddReminder()
    {
        try
        {
            await Navigator.PushAsync(new ReminderPage());
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private void FilterByDate()
    {
        try
        {
            if (DateTime.Now <= ReminderDateFilter)
            {
                FilteredReminders = Reminders.Where(x => x.ReminderDate.Date == ReminderDateFilter.Date).ToObservableCollection();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private void FilterByNameDescriptionOrLocation()
    {
        try
        {
            FilteredReminders = Reminders.Where(x => x.ReminderLocation.Contains(ReminderFilter, StringComparison.InvariantCultureIgnoreCase) || x.ReminderDescription.Contains(ReminderFilter, StringComparison.InvariantCultureIgnoreCase) || x.ReminderName.Contains(ReminderFilter, StringComparison.InvariantCultureIgnoreCase)).ToObservableCollection();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private async Task DeleteSelectedReminder(ReminderResponse reminder)
    {
        try
        {
            var reminderId = await stringStorage.GetAsync(stringStorage.ReminderId);
            var response = await mediator.Send(new Application.Requests.Reminders.DeleteReminderRequest()
            {
                Id = reminderId
            });
            if (response.Success)
            {
                await CurrentPage.DisplayAlert(Title, "Lembrete removido com sucesso", "Ok");
                RefreshRemindersCommand.Execute(response);
            }
            else
            {
                await CurrentPage.DisplayAlert(Title, $"Lembrete nao removido. Motivo: {response.Message}", "Ok");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private async Task EditSelectedReminder(ReminderResponse reminder)
    {
        try
        {
            await stringStorage.SetAsync(stringStorage.ReminderId, reminder.ReminderId);
            await Navigator.PushAsync(new EditReminderPage());
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private void TextHasChanged()
    {
        try
        {
            if (string.IsNullOrEmpty(ReminderFilter))
            {
                FilteredReminders = Reminders;
                return;
            }
            FilterByNameDescriptionOrLocationCommand.Execute(this);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private async Task RefreshReminders()
    {
        try
        {
            IsRefreshing = true;
            await LoadReminders();
            FilteredReminders = Reminders;
            IsRefreshing = false;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }


    [RelayCommand]
    private void RemoveFilters()
    {
        try
        {
            FilteredReminders = Reminders;
            ReminderDateFilter = DateTime.Today;
            ReminderFilter = string.Empty;
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    [RelayCommand]
    private async Task ViewProfile()
    {
        try
        {
            await Navigator.PushAsync(new ProfilePage());
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
    }

    #endregion

}
