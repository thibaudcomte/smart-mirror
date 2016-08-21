using SmartMirror.App.Services;
using System;
using System.Threading.Tasks;

namespace SmartMirror.App.Models
{
    public class Clock : IDataProvider
    {
        public async Task<bool> OpenAsync()
        {
            Refresh();

            TimerServices.Instance.NewMinuteNotified += NewMinuteNotified;

            return await Task.FromResult(true);
        }

        public void Refresh()
        {
            CurrentDateTime = TimerServices.Instance.DateTime;
            CurrentDateTimeChanged?.Invoke(this, EventArgs.Empty);
        }

        public DateTime CurrentDateTime { get; private set; }

        public event EventHandler CurrentDateTimeChanged;

        #region internals

        private void NewMinuteNotified(object sender, EventArgs e)
        {
            Refresh();
        }

        #endregion
    }
}
