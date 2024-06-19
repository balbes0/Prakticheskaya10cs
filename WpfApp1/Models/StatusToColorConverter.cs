using System.Globalization;
using System.Windows.Data;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int statusId = (int)value;
        return statusId switch
        {
            1 => "#007BFE", // Текущая запись
            2 => "#E8EAED", // Завершенная запись
            _ => "#807F7F", // Пропущенная запись
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
