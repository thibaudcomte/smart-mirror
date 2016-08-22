using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartMirror.App.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace SmartMirror.App.Models
{
    public class WeatherMetrics
    {
        public string Description { get; set; }
        public double Temp { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int Cloudiness { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public DateTimeOffset SunsetTime { get; set; }
        public DateTimeOffset SunriseTime { get; set; }
        public bool SunTimeAvailable { get; set; } = true;
        public string IconName { get; set; }
    }

    public class WeatherMetricsList : List<WeatherMetrics> { }

    public class Weather : IDataProvider
    {
        public async Task<bool> OpenAsync()
        {
            TimerServices.Instance.NewHourNotified += NewHourNotified;

            const string settingsFileName = @"ms-appx:///settings.json";
            var fileUri = new Uri(settingsFileName);

            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
                string text = await FileIO.ReadTextAsync(file);
                var json = (JObject)JsonConvert.DeserializeObject(text);

                var appIdKey = json["weather"]["key"].Value<string>();
                var city = json["weather"]["city"].Value<string>();
                var country = json["weather"]["country"].Value<string>();

                _UriCurrent = $@"http://api.openweathermap.org/data/2.5/weather?q={city},{country}&units=metric&appId={appIdKey}";
                _UriForecast = $@"http://api.openweathermap.org/data/2.5/forecast?q={city},{country}&units=metric&cnt={_ResultCount}&appId={appIdKey}";
            }
            catch (Exception)
            {
                return false;
            }

            return await Update();
        }

        public void Refresh()
        {
#pragma warning disable CS4014
            Update();
#pragma warning restore CS4014
        }

        public WeatherMetrics TodayMetrics { get; private set; }

        public WeatherMetricsList WeekForecasts { get; private set; } = new WeatherMetricsList();

        public event EventHandler WeatherChanged;


        #region internals

        private string _UriCurrent;
        private string _UriForecast;
        private const int _ResultCount = 4;

        private async Task<bool> Update()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // current weather
                    TodayMetrics = null;
                    var text = await client.GetStringAsync(_UriCurrent);
                    var json = (JObject)JsonConvert.DeserializeObject(text);

                    if (json["cod"].Value<int>() == 200)
                    {
                        TodayMetrics = new WeatherMetrics();
                        TodayMetrics.Description = json["weather"][0]["description"].Value<string>();
                        TodayMetrics.Temp = json["main"]["temp"].Value<double>();
                        TodayMetrics.Humidity = json["main"]["humidity"].Value<int>();
                        TodayMetrics.WindSpeed = json["wind"]["speed"].Value<double>();
                        TodayMetrics.Cloudiness = json["clouds"]["all"].Value<int>();
                        TodayMetrics.DateTime = DateTimeOffset.FromUnixTimeSeconds(json["dt"].Value<long>());
                        TodayMetrics.SunriseTime = DateTimeOffset.FromUnixTimeSeconds(json["sys"]["sunrise"].Value<long>());
                        TodayMetrics.SunsetTime = DateTimeOffset.FromUnixTimeSeconds(json["sys"]["sunset"].Value<long>());
                        TodayMetrics.IconName = json["weather"][0]["icon"].Value<string>();
                    }

                    // 5-day weather forecast
                    WeekForecasts.Clear();
                    text = await client.GetStringAsync(_UriForecast);
                    json = (JObject)JsonConvert.DeserializeObject(text);

                    if (json["cod"].Value<int>() == 200)
                    {
                        var count = Math.Min(_ResultCount, json["cnt"].Value<int>());
                        for (int i = 0; i < count; i++)
                        {
                            WeekForecasts.Add(new WeatherMetrics
                            {
                                Temp = json["list"][i]["main"]["temp"].Value<double>(),
                                Humidity = json["list"][i]["main"]["humidity"].Value<int>(),
                                Description = json["list"][i]["weather"][0]["description"].Value<string>(),
                                Cloudiness = json["list"][i]["clouds"]["all"].Value<int>(),
                                WindSpeed = json["list"][i]["wind"]["speed"].Value<double>(),
                                DateTime = DateTimeOffset.FromUnixTimeSeconds(json["list"][i]["dt"].Value<long>()),
                                IconName = json["list"][i]["weather"][0]["icon"].Value<string>(),
                                SunTimeAvailable = false
                            });
                        }
                    }
                }
            }
            catch { }

            WeatherChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        private async void NewHourNotified(object sender, EventArgs e)
        {
            await Update();
        }

        #endregion
    }
}
