using System;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace SmartMirror.App.Services
{
    public class TimerServices
    {
        public static TimerServices Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TimerServices();
                return _instance;
            }
        }

        public DateTime DateTime { get; private set; }

        public event EventHandler NewMinuteNotified;
        public event EventHandler NewHourNotified;
        public event EventHandler NewDayNotified;

        public void Sync()
        {
            DateTime = DateTime.Now;

            NotifyNewMinute();
            NotifyNewHour();
            NotifyNewDay();

            DateTime = DateTime.Now;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            RunUpdateLoop(60 - DateTime.Second);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        }

        #region internals

        private TimerServices()
        {
            Sync();
        }

        private async Task RunUpdateLoop(int seconds)
        {
            await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds));
                UpdateWhenNecessary();
            });

            _timer = ThreadPoolTimer.CreatePeriodicTimer(
                (source) =>
                {
                    Task.Run(() =>
                    {
                        UpdateWhenNecessary();
                    });
                },
                TimeSpan.FromSeconds(60));
        }

        private void UpdateWhenNecessary()
        {
            var cdt = DateTime.Now;

            var needMinuteUpdate = cdt.Minute != DateTime.Minute;
            var needHourUpdate = cdt.Hour != DateTime.Hour;
            var needDayUpdate = cdt.Day != DateTime.Day;

            DateTime = cdt;

            if (needMinuteUpdate) NotifyNewMinute();
            if (needHourUpdate) NotifyNewHour();
            if (needDayUpdate) NotifyNewDay();
        }

        private void NotifyNewMinute()
        {
            NewMinuteNotified?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyNewHour()
        {
            NewHourNotified?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyNewDay()
        {
            NewDayNotified?.Invoke(this, EventArgs.Empty);
        }

        private static TimerServices _instance;

        private ThreadPoolTimer _timer;

        #endregion
    }
}
