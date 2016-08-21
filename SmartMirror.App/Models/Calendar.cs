using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMirror.App.Models
{
    public class CalendarStores : List<CalendarStore> { }

    public class CalendarStore
    {
        public string Name { get; set; }
        public Windows.UI.Color Color { get; set; }
        public CalendarEntries Entries { get; set; } = new CalendarEntries();
    }

    public class CalendarEntries : List<CalendarEntry> { }

    public class CalendarEntry
    {
        public string Subject { get; set; }
        public DateTimeOffset Time { get; set; }
    }

    public class Calendar
    {
        public Calendar()
        {
            Initialize();
        }

        private async void Initialize()
        {
            _store = await Windows.ApplicationModel.Appointments.AppointmentManager.
                RequestStoreAsync(Windows.ApplicationModel.Appointments.AppointmentStoreAccessType.AllCalendarsReadOnly);

            await Update();

            _store.StoreChanged += _store_StoreChanged;
        }

        private async void _store_StoreChanged(Windows.ApplicationModel.Appointments.AppointmentStore sender, Windows.ApplicationModel.Appointments.AppointmentStoreChangedEventArgs args)
        {
            var deferral = args.GetDeferral();
            await Update();
            deferral.Complete();
        }

        private Windows.ApplicationModel.Appointments.AppointmentStore _store;

        public CalendarStores CalendarStores { get; } = new CalendarStores();

        public async Task Update()
        {
            var calendars = await _store.FindAppointmentCalendarsAsync();

            CalendarStores.Clear();

            foreach (var cal in calendars)
            {
                var appointments = await cal.FindAppointmentsAsync(DateTimeOffset.Now.AddDays(-1), TimeSpan.FromDays(14));

                if (appointments.Any())
                {
                    var calStore = new CalendarStore
                    {
                        Name = cal.DisplayName,
                        Color = cal.DisplayColor
                    };

                    foreach (var apt in appointments)
                    {
                        calStore.Entries.Add(new CalendarEntry
                        {
                            Subject = apt.Subject,
                            Time = apt.StartTime
                        });
                    }

                    CalendarStores.Add(calStore);
                }
            }

            CalendarChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CalendarChanged;
    }
}
