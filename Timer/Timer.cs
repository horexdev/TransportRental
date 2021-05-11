using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace TransportRental.Timer {
    public class Timer {
        private static readonly Dictionary<string, DispatcherTimer> Timers = new Dictionary<string, DispatcherTimer>();

        /// <summary>
        /// Запустить таймер
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="action"></param>
        /// <param name="name"></param>
        public static void StartTimer(string name, int interval, Action action)
        {
            var timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, interval)
            };

            timer.Tick += (sender, args) =>
            {
                action.Invoke();
            };

            timer.Start();

            Timers.Add(name, timer);
        }

        /// <summary>
        /// Остановить таймер
        /// </summary>
        /// <param name="name"></param>
        public static void StopTimer(string name) {
            var timer = Timers.FirstOrDefault(t => t.Key == name).Value;

            timer?.Stop();
			
			Timers.Remove(name);
        }
    }
}