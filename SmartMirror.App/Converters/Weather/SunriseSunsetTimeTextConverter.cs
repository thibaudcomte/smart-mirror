using System;
using Windows.UI.Xaml.Data;

namespace SmartMirror.App.Converters.Weather
{
    public class SunriseSunsetTimeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dt = (DateTimeOffset)value;
            return dt.ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
