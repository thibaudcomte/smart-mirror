using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartMirror.App.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace SmartMirror.App.Models
{
    public class Ephemeris : IDataProvider
    {
        public async Task<bool> OpenAsync()
        {
            const string fileName = @"ms-appx:///ephemeris.json";
            var fileUri = new Uri(fileName);

            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
                string text = await FileIO.ReadTextAsync(file);
                _jsonObject = (JObject)JsonConvert.DeserializeObject(text);
            }
            catch (Exception)
            {
                return false;
            }

            Refresh();

            TimerServices.Instance.NewDayNotified += NewDayNotified;

            return true;
        }

        public void Refresh()
        {
            CurrentEphemeris = GetEphemeris(TimerServices.Instance.DateTime);
            CurrentEphemerisChanged?.Invoke(this, EventArgs.Empty);
        }

        public string CurrentEphemeris { get; private set; }

        public event EventHandler CurrentEphemerisChanged;


        #region internals

        private string GetEphemeris(DateTime date)
        {
            var monthLiterals = new Dictionary<int, string>
            {
                {1, "january" },
                {2, "february" },
                {3, "march" },
                {4, "april" },
                {5, "may" },
                {6, "june" },
                {7, "july" },
                {8, "august" },
                {9, "september" },
                {10, "october" },
                {11, "november" },
                {12, "december" }
            };

            var data = _jsonObject[monthLiterals[date.Month]][date.Day - 1];

            var prefix = data[1].Value<string>();
            var name = data[0].Value<string>();

            if (prefix.Length == 0)
                return name;

            return $"{prefix} {name}";
        }

        private void NewDayNotified(object sender, EventArgs e)
        {
            Refresh();
        }

        private JObject _jsonObject;

        #endregion
    }
}
