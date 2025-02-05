using MReminders.Mobile.Client.ViewModels;

namespace MReminders.Mobile.Client.Views;

public partial class MainPage : ContentPage
{ 
    public MainPage()
    {
        InitializeComponent();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((MainViewModel)BindingContext).RemoveFiltersCommand.Execute(null);
    }
}
