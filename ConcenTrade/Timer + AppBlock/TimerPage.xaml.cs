using Concentrade;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Concentrade
{
    public partial class TimerPage : Page
    {
        private DispatcherTimer _timer;
        private TimeSpan _remaining;
        private AppBlocker _blocker = new AppBlocker();

        public TimerPage(int dureeMinutes)
        {
            InitializeComponent();
            _remaining = TimeSpan.FromMinutes(dureeMinutes);

            _blocker.Start(); // 👈 démarre la surveillance

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            UpdateTimerText();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));

            if (_remaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                TimerText.Text = "Terminé 🎉";
                // Tu peux ajouter ici une redirection ou son, etc.
            }
            else
            {
                UpdateTimerText();
            }
        }

        private void UpdateTimerText()
        {
            TimerText.Text = _remaining.ToString(@"hh\:mm\:ss");

        }
    }
}
