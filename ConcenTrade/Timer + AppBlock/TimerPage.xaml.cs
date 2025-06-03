using Concentrade;
using System;
using System.Diagnostics;
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

            // Démarrer le bloqueur
            _blocker.Start();

            // Afficher la fenêtre de confirmation pour les applications déjà lancées
            var confirmationWindow = new DistractingAppsConfirmation(_blocker);
            if (confirmationWindow.ShowDialog() == true && !confirmationWindow.ContinueWithoutClosing)
            {
                foreach (var app in confirmationWindow.RunningApps)
                {
                    if (app.IsSelected)
                    {
                        try { app.Process.Kill(); } catch { }
                    }
                }
            }

            // Lancer le timer
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

                // Arrêter le blocage
                _blocker.Stop();
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
