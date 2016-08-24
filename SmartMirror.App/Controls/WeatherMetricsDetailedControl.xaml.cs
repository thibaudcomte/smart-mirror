using SmartMirror.App.Models;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SmartMirror.App.Controls
{
    public sealed partial class WeatherMetricsDetailedControl : UserControl
    {
        public WeatherMetricsDetailedControl()
        {
            this.InitializeComponent();
        }

        public WeatherMetrics WeatherMetrics
        {
            get { return (WeatherMetrics)GetValue(WeatherMetricsProperty); }
            set { SetValue(WeatherMetricsProperty, value); }
        }

        public static readonly DependencyProperty WeatherMetricsProperty =
            DependencyProperty.Register("WeatherMetrics", typeof(WeatherMetrics),
                typeof(WeatherMetricsDetailedControl), new PropertyMetadata(null));
    }
}
