using System;
using Windows.UI.Xaml.Data;

namespace SmartMirror.App.Converters.Weather
{
    public class DateTimeOffsetTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dt = (DateTimeOffset)value;
            var time = dt.ToString("HH:mm");

            if (dt.Day == DateTime.Today.Day)
            {
                if (dt.Hour < 18)
                    return $"today, {time}";
                else if (dt.Hour >= 18)
                    return $"tonight, {time}";
            }
            else if (dt.Day == DateTime.Today.AddDays(1).Day)
            {
                return $"tomorrow, {time}";
            }

            return dt.ToString("d") + ", " + time;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
