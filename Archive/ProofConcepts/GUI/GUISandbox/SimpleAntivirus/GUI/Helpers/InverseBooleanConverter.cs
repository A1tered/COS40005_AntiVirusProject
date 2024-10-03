using System.Globalization;
using System.Windows.Data;


/// <summary>
/// This converter inverts the value of a boolean
/// So if it's true, converts to false, and vice versa
/// </summary>
/// 
namespace SimpleAntivirus.GUI.Helpers
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object paramater, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
