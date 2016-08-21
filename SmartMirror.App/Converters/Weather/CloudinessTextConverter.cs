using System;
using Windows.UI.Xaml.Data;

namespace SmartMirror.App.Converters.Weather
{
    public class CloudinessTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cloudiness = (int)value;
            return $"{cloudiness} %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
