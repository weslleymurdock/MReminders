using System.Collections.ObjectModel;
using System.Globalization;

namespace MReminders.Mobile.Client;

public class DateVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var date = parameter as DateTime?;

        if (value is not ObservableCollection<DateTime> reminderDates || date == null)
        {
            return false;
        }

        return reminderDates.Contains(date.Value);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}