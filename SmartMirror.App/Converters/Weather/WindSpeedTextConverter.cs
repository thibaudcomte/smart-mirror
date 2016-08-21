using System;
using Windows.UI.Xaml.Data;

namespace SmartMirror.App.Converters.Weather
{
    public class WindSpeedTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var speedInMeterPerSecond = (double)value;
            var speedInKmsPerHour = speedInMeterPerSecond * 3.6;

            return speedInKmsPerHour.ToString("F0") + " km/h";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
