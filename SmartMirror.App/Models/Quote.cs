using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartMirror.App.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartMirror.App.Models
{
    public class QuoteEntry
    {
        public string Text { get; set; }
        public string Source { get; set; }
    }

    public class Quote : IDataProvider
    {
        public async Task<bool> OpenAsync()
        {
            TimerServices.Instance.NewDayNotified += NewDayNotified;
            return await Update();
        }

        public void Refresh()
        {
#pragma warning disable CS4014
            Update();
#pragma warning restore CS4014
        }

        public QuoteEntry QuoteOfTheDay { get; private set; } = new QuoteEntry();

        public event EventHandler QuoteOfTheDayChanged;

        #region internals

        private const string _Uri = @"http://quotes.rest/qod.json";

        private async Task<bool> Update()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var text = await client.GetStringAsync(_Uri);
                    var json = (JObject)JsonConvert.DeserializeObject(text);

                    QuoteOfTheDay.Text = json["contents"]["quotes"][0]["quote"].Value<string>();
                    QuoteOfTheDay.Source = json["contents"]["quotes"][0]["author"].Value<string>();
                }
            }
            catch (Exception)
            {
                return false;
            }

            QuoteOfTheDayChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        private async void NewDayNotified(object sender, EventArgs e)
        {
            await Update();
        }

        #endregion
    }
}
