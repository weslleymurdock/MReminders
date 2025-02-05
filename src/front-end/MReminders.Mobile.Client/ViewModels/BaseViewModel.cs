using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.Configuration;
using MReminders.Mobile.Infrastructure.Interfaces;
using MReminders.Rest.Client;
using System.Collections.ObjectModel;

namespace MReminders.Mobile.Client.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    #region Services 
    protected IMediator mediator;
    protected IIdentityService identity;
    protected IPermissionsService permissions;
    protected IBiometricsService biometrics;
    protected ITokenStorage tokenStorage; 
    protected ITokenRenewalService tokenRenewal;
    protected IConfiguration configuration;
    protected IProtectedStorage<string> stringStorage;
    protected IRemindersService remindersService;
    #endregion

    #region Observable Properties
    [ObservableProperty]
    private string title = string.Empty;
    [ObservableProperty]
    private ObservableCollection<ReminderResponse> reminders = [];
    [ObservableProperty]
    private ObservableCollection<DateTime> reminderDates = [];

    [ObservableProperty]
    private ObservableCollection<DateTime> daysInMonth = [];
    #endregion

    #region Protected Properties
    protected Shell Shell { get => Shell.Current; }
    protected IServiceProvider Services { get => Shell.Handler!.MauiContext!.Services; }
    protected INavigation Navigator { get => this.CurrentPage.Navigation; }
    protected Page CurrentPage { get => Shell.CurrentPage; }
    protected Microsoft.Maui.Controls.Application CurrentApplication { get => Microsoft.Maui.Controls.Application.Current!; }
    #endregion

    public BaseViewModel()
    {
        mediator = Services.GetRequiredService<IMediator>();
        identity = Services.GetRequiredService<IIdentityService>();
        biometrics = Services.GetRequiredService<IBiometricsService>();
        permissions = Services.GetRequiredService<IPermissionsService>(); 
        tokenRenewal = Services.GetRequiredService<ITokenRenewalService>();
        tokenStorage = Services.GetRequiredService<ITokenStorage>();
        configuration = Services.GetRequiredService<IConfiguration>();
        stringStorage = Services.GetRequiredService<IProtectedStorage<string>>();
        remindersService = Services.GetRequiredService<IRemindersService>();
    }

    protected async Task LoadReminders(CancellationToken token = default)
    {
        var userKey = await stringStorage.GetAsync(stringStorage.UserKey);
        Reminders = (await remindersService.GetRemindersFromUserAsync(userKey, token)).Data.ToObservableCollection();

        foreach (var reminder in Reminders)
        {
            ReminderDates.Add(reminder.ReminderDate);
        }

        LoadCalendar();
    }
    private void LoadCalendar()
    {
        DaysInMonth.Clear();
        var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            var currentDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
            DaysInMonth.Add(currentDay);
        }
    }
}