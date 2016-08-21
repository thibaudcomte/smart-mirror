using System;
using Windows.UI.Xaml.Data;

namespace SmartMirror.App.Converters.Weather
{
    public class IconNameToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var name = value.ToString();
            return $"ms-appx:///Icons/Weather/{name}.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
