using System.Globalization;
using System.Windows.Data;


/// <summary>
/// This converter inverts the value of a boolean AND assigns that to a visibility value
/// So if it's true, converts to false, and vice versa
/// </summary>
/// 
namespace SimpleAntivirus.GUI.Helpers
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible; // Default is visible if value isn't a boolean
        }

        public object ConvertBack(object value, Type targetType, object paramater, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }

            return false;
        }
    }
}