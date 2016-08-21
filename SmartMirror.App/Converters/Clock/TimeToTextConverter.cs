using System;
using Windows.UI.Xaml.Data;

namespace SmartMirror.App.Converters.Clock
{
    public class TimeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dt = (DateTime)value;
            return dt.ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
