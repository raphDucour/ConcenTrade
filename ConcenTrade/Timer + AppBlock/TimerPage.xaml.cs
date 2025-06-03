using Concentrade;
using System;
using System.Collections.Generic;
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
        private Dictionary<string, DispatcherTimer> _temporaryAllowanceTimers = new();
        private bool _isPaused = false;

        public TimerPage(int dureeMinutes)
        {
            InitializeComponent();
            _remaining = TimeSpan.FromMinutes(dureeMinutes);

            // S'abonner à l'événement d'autorisation temporaire
            _blocker.OnTemporaryAllowance += Blocker_OnTemporaryAllowance;

            // Démarrer le bloqueur
            _blocker.Start();

            // Utiliser Dispatcher.BeginInvoke pour s'assurer que la fenêtre est complètement initialisée
            Dispatcher.BeginInvoke(new Action(async () =>
            {
                // Afficher la fenêtre de confirmation pour les applications déjà lancées
                var confirmationWindow = new RunningAppsPopup(_blocker);
                if (confirmationWindow.ShowDialog() == true && !confirmationWindow.ContinueWithoutClosing)
                {
                    foreach (var app in confirmationWindow.RunningApps)
                    {
                        if (app.IsSelected && app.Process != null)
                        {
                            try 
                            { 
                                app.Process.Kill();
                                // Attendre un peu pour s'assurer que le processus est bien terminé
                                await System.Threading.Tasks.Task.Delay(100);
                            }
                            catch { }
                        }
                    }
                }

                // Lancer le timer
                StartTimer();
            }));
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            UpdateTimerText();
        }

        private void Blocker_OnTemporaryAllowance(object? sender, TemporaryAllowanceEventArgs e)
        {
            // Mettre le timer en pause
            _timer.Stop();
            _isPaused = true;

            // Mettre à jour l'affichage pour montrer que le timer est en pause
            Application.Current.Dispatcher.Invoke(() =>
            {
                TimerText.Text += " (Pause)";
            });

            // Créer un timer pour la durée de l'autorisation
            var allowanceTimer = new DispatcherTimer();
            allowanceTimer.Interval = TimeSpan.FromSeconds(1);
            var remainingAllowance = e.Duration;

            allowanceTimer.Tick += (s, args) =>
            {
                remainingAllowance = remainingAllowance.Subtract(TimeSpan.FromSeconds(1));
                
                if (remainingAllowance.TotalSeconds <= 0)
                {
                    // L'autorisation est terminée
                    allowanceTimer.Stop();
                    _temporaryAllowanceTimers.Remove(e.ProcessName);

                    // Si c'était le dernier timer d'autorisation, reprendre le timer principal
                    if (_temporaryAllowanceTimers.Count == 0 && _isPaused)
                    {
                        _isPaused = false;
                        _timer.Start();
                        UpdateTimerText();
                    }
                }
            };

            _temporaryAllowanceTimers[e.ProcessName] = allowanceTimer;
            allowanceTimer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));

            if (_remaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                TimerText.Text = "Terminé 🎉";

                // Arrêter le blocage et tous les timers d'autorisation
                _blocker.Stop();
                foreach (var timer in _temporaryAllowanceTimers.Values)
                {
                    timer.Stop();
                }
                _temporaryAllowanceTimers.Clear();
            }
            else
            {
                UpdateTimerText();
            }
        }

        private void UpdateTimerText()
        {
            TimerText.Text = _remaining.ToString(@"hh\:mm\:ss");
            if (_isPaused)
            {
                TimerText.Text += " (Pause)";
            }
        }
    }
}
