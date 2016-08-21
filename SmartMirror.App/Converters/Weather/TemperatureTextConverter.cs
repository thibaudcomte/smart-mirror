using System;
using Windows.UI.Xaml.Data;

namespace SmartMirror.App.Converters.Weather
{
    public class TemperatureTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((double)value).ToString("F0") + "ºC";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
