using Concentrade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Concentrade
{
    public partial class TimerPage : Page
    {
        private DispatcherTimer _timer;
        private TimeSpan _remaining;
        private AppBlocker _blocker;
        private Dictionary<string, DispatcherTimer> _temporaryAllowanceTimers = new();
        private bool _isPaused = false;

        public TimerPage(int dureeMinutes)
        {
            InitializeComponent();
            _remaining = TimeSpan.FromMinutes(dureeMinutes);

            // Utiliser l'instance globale de AppBlocker
            _blocker = ((App)Application.Current).AppBlocker;

            // S'abonner à l'événement d'autorisation temporaire
            _blocker.OnTemporaryAllowance += Blocker_OnTemporaryAllowance;

            // Activer le blocage
            _blocker.SetActive(true);

            // Utiliser Dispatcher.BeginInvoke pour s'assurer que la fenêtre est complètement initialisée
            Dispatcher.BeginInvoke(new Action(() =>
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
                                System.Threading.Thread.Sleep(100);
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

                    // Afficher le popup de fin de temps
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var popup = new TimeUpPopup(e.ProcessName);
                        if (popup.ShowDialog() == true)
                        {
                            switch (popup.Action)
                            {
                                case TimeUpPopup.TimeUpAction.Prolonger:
                                    // Prolonger de 5 minutes
                                    _blocker.AllowTemporarily(e.ProcessName, TimeSpan.FromMinutes(5));
                                    break;

                                case TimeUpPopup.TimeUpAction.Reprendre:
                                    // Si c'était le dernier timer d'autorisation, reprendre le timer principal
                                    if (_temporaryAllowanceTimers.Count == 0 && _isPaused)
                                    {
                                        _isPaused = false;
                                        _timer.Start();
                                        UpdateTimerText();
                                    }
                                    break;

                                case TimeUpPopup.TimeUpAction.Fermer:
                                    try
                                    {
                                        // Trouver et tuer le processus
                                        var processes = Process.GetProcessesByName(e.ProcessName);
                                        foreach (var process in processes)
                                        {
                                            process.Kill(true);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"Erreur lors de la fermeture de {e.ProcessName}: {ex.Message}");
                                    }

                                    // Si c'était le dernier timer d'autorisation, reprendre le timer principal
                                    if (_temporaryAllowanceTimers.Count == 0 && _isPaused)
                                    {
                                        _isPaused = false;
                                        _timer.Start();
                                        UpdateTimerText();
                                    }
                                    break;
                            }
                        }
                    });
                }
            };

            _temporaryAllowanceTimers[e.ProcessName] = allowanceTimer;
            allowanceTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));

            if (_remaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                TimerText.Text = "Terminé 🎉";

                // Désactiver le blocage et arrêter tous les timers d'autorisation
                _blocker.SetActive(false);
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

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPaused)
            {
                // Reprendre le timer
                _timer.Start();
                _isPaused = false;
                PauseButton.Content = "⏯️ Pause";
            }
            else
            {
                // Mettre en pause
                _timer.Stop();
                _isPaused = true;
                PauseButton.Content = "⏯️ Reprendre";
            }
            UpdateTimerText();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Arrêter le timer
            _timer.Stop();
            
            // Désactiver le blocage et arrêter tous les timers d'autorisation
            _blocker.SetActive(false);
            foreach (var timer in _temporaryAllowanceTimers.Values)
            {
                timer.Stop();
            }
            _temporaryAllowanceTimers.Clear();

            // Retourner à la page principale
            await Task.Run(() => 
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    NavigationService?.Navigate(new MenuPage());
                });
            });
        }
    }
}
