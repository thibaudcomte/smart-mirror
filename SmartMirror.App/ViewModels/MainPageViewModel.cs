using SmartMirror.App.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SmartMirror.App.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged, IDataProvider
    {
        public MainPageViewModel()
        {
            _clock = new Clock();
            _clock.CurrentDateTimeChanged += CurrentDateTimeChanged;

            _ephemeris = new Ephemeris();
            _ephemeris.CurrentEphemerisChanged += CurrentEphemerisChanged;

            _quote = new Quote();
            _quote.QuoteOfTheDayChanged += QuoteOfTheDayChanged;

            ForecastWeatherMetrics = new ObservableCollection<WeatherMetrics>();

            _weather = new Weather();
            _weather.WeatherChanged += WeatherChanged;
        }

        public async Task<bool> OpenAsync()
        {
            await Task.WhenAll(_clock.OpenAsync(), _ephemeris.OpenAsync(), _quote.OpenAsync(), _weather.OpenAsync());
            return true;
        }

        public void Refresh()
        {
            _clock.Refresh();
            _ephemeris.Refresh();
            _quote.Refresh();
            _weather.Refresh();
        }

        public DateTime CurrentDateTime
        {
            get { return _currentDateTime; }
            private set { _currentDateTime = value; RaisePropertyChanged(nameof(CurrentDateTime)); }
        }
        private DateTime _currentDateTime;

        public string CurrentEphemeris
        {
            get { return _currentEphemeris; }
            private set { _currentEphemeris = value; RaisePropertyChanged(nameof(CurrentEphemeris)); }
        }
        private string _currentEphemeris;

        public QuoteEntry QuoteOfTheDay
        {
            get { return _quoteOfTheDay; }
            set { _quoteOfTheDay = value; RaisePropertyChanged(nameof(QuoteOfTheDay)); }
        }
        private QuoteEntry _quoteOfTheDay;

        public WeatherMetrics CurrentWeatherMetrics
        {
            get { return _currentWeatherMetrics; }
            private set { _currentWeatherMetrics = value; RaisePropertyChanged(nameof(CurrentWeatherMetrics)); }
        }
        private WeatherMetrics _currentWeatherMetrics;

        public ObservableCollection<WeatherMetrics> ForecastWeatherMetrics { get; private set; }

        #region internals

        private async void CurrentDateTimeChanged(object sender, EventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.High, () => { CurrentDateTime = _clock.CurrentDateTime; });
        }

        private async void CurrentEphemerisChanged(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () => { CurrentEphemeris = _ephemeris.CurrentEphemeris; });
        }

        private async void QuoteOfTheDayChanged(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () => { QuoteOfTheDay = _quote.QuoteOfTheDay; });
        }

        private async void WeatherChanged(object sender, EventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    CurrentWeatherMetrics = _weather.TodayMetrics;
                    ForecastWeatherMetrics.Clear();
                    _weather.WeekForecasts.ForEach((i) => { ForecastWeatherMetrics.Add(i); });
                });
        }

        private Clock _clock;
        private Ephemeris _ephemeris;
        private Quote _quote;
        private Weather _weather;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
