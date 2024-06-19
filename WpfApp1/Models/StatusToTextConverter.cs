using System.Globalization;
using System.Windows.Data;

public class StatusToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int statusId = (int)value;
        return statusId switch
        {
            1 => "Начать прием",
            2 => "Запись завершена",
            _ => "Пропущенная запись",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}