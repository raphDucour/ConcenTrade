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
        private enum PomodoroState { Work, ShortBreak, Finished }

        private DispatcherTimer _timer;
        private TimeSpan _remaining;
        private AppBlocker _blocker;
        private Dictionary<string, DispatcherTimer> _temporaryAllowanceTimers = new();
        private bool _isPaused = false;
        private int _pointsAccumules = 0;
        private TextBlock _pointsText;

        private PomodoroState _currentState;
        private int _totalCycles;
        private int _currentCycle;
        private readonly TimeSpan _workDuration = TimeSpan.FromMinutes(25);
        private readonly TimeSpan _breakDuration = TimeSpan.FromMinutes(5);

        public TimerPage(int cycles)
        {
            InitializeComponent();
            _totalCycles = cycles;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;

            _pointsText = new TextBlock
            {
                FontSize = 20,
                Foreground = System.Windows.Media.Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 20, 20, 0)
            };
            UpdatePointsText();
            MainGrid.Children.Add(_pointsText);

            _blocker = ((App)Application.Current).AppBlocker;
            _blocker.OnTemporaryAllowance += Blocker_OnTemporaryAllowance;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var confirmationWindow = new RunningAppsPopup(_blocker);
                if (confirmationWindow.ShowDialog() == true && !confirmationWindow.ContinueWithoutClosing)
                {
                    foreach (var app in confirmationWindow.RunningApps)
                    {
                        if (app.IsSelected && app.Process != null)
                        {
                            try { app.Process.Kill(); System.Threading.Thread.Sleep(100); }
                            catch { }
                        }
                    }
                }
                StartPomodoro();
            }));
        }

        private void ResumeMainTimerIfNeeded()
        {
            // Reprend le minuteur principal seulement si aucune autre app n'est autorisée.
            if (_temporaryAllowanceTimers.Count == 0 && _isPaused)
            {
                _isPaused = false;
                _timer.Start();
                UpdateTimerText();
            }
        }

        // =========================================================================
        //  LA LOGIQUE QUI CORRIGE LA BOUCLE EST DANS CETTE MÉTHODE
        // =========================================================================
        private void Blocker_OnTemporaryAllowance(object? sender, TemporaryAllowanceEventArgs e)
        {
            if (_currentState != PomodoroState.Work) return;

            if (!_isPaused)
            {
                _timer.Stop();
                _isPaused = true;
                Application.Current.Dispatcher.Invoke(UpdateTimerText);
            }

            // Création du minuteur pour la PREMIÈRE autorisation
            var firstAllowanceTimer = new DispatcherTimer { Interval = e.Duration };
            firstAllowanceTimer.Tick += (s, args) =>
            {
                firstAllowanceTimer.Stop();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Process[] processes = Process.GetProcessesByName(e.ProcessName);
                    if (processes.Length > 0)
                    {
                        Process processToBlock = processes[0];
                        var popup = new TimeUpPopup(e.ProcessName, processToBlock);

                        if (popup.ShowDialog() == true)
                        {
                            if (popup.Action == TimeUpPopup.TimeUpAction.Extend)
                            {
                                // *** LA CORRECTION EST ICI ***
                                // On crée un minuteur final et ISOLÉ.
                                // Il ne redéclenche PAS l'événement OnTemporaryAllowance.
                                var finalAllowanceTimer = new DispatcherTimer { Interval = popup.ExtensionDuration };
                                finalAllowanceTimer.Tick += (s2, args2) =>
                                {
                                    finalAllowanceTimer.Stop();
                                    // Le temps est écoulé : on ferme l'app et on tente de reprendre le timer.
                                    try { if (!processToBlock.HasExited) processToBlock.Kill(true); } catch { }

                                    _temporaryAllowanceTimers.Remove(e.ProcessName);
                                    ResumeMainTimerIfNeeded();
                                };

                                _temporaryAllowanceTimers[e.ProcessName] = finalAllowanceTimer;
                                finalAllowanceTimer.Start();
                            }
                            else // L'utilisateur a choisi "Fermer maintenant"
                            {
                                try { if (!processToBlock.HasExited) processToBlock.Kill(true); } catch { }
                                _temporaryAllowanceTimers.Remove(e.ProcessName);
                                ResumeMainTimerIfNeeded();
                            }
                        }
                        else // L'utilisateur a fermé le popup sans choisir
                        {
                            try { if (!processToBlock.HasExited) processToBlock.Kill(true); } catch { }
                            _temporaryAllowanceTimers.Remove(e.ProcessName);
                            ResumeMainTimerIfNeeded();
                        }
                    }
                    else
                    {
                        _temporaryAllowanceTimers.Remove(e.ProcessName);
                        ResumeMainTimerIfNeeded();
                    }
                });
            };
            _temporaryAllowanceTimers[e.ProcessName] = firstAllowanceTimer;
            firstAllowanceTimer.Start();
        }

        // --- Le reste du fichier ne change pas ---
        #region Méthodes de gestion du minuteur principal
        private void StartPomodoro()
        {
            _currentCycle = 1;
            StartWorkSession();
        }

        private void StartWorkSession()
        {
            _currentState = PomodoroState.Work;
            _remaining = _workDuration;
            _isPaused = false;
            PauseButton.Content = "⏯️ Pause";
            _blocker.SetActive(true);
            UpdateTimerText();
            _timer.Start();
        }

        private void StartBreakSession()
        {
            _currentState = PomodoroState.ShortBreak;
            _remaining = _breakDuration;
            _isPaused = false;
            PauseButton.Content = "⏯️ Pause";
            _blocker.SetActive(false);
            foreach (var timer in _temporaryAllowanceTimers.Values) timer.Stop();
            _temporaryAllowanceTimers.Clear();
            UpdateTimerText();
            _timer.Start();
        }

        private void FinishSession()
        {
            _timer.Stop();
            _currentState = PomodoroState.Finished;
            StateText.Text = "Félicitations !";
            TimerText.Text = "🎉";
            SavePoints();
            _blocker.SetActive(false);
            foreach (var timer in _temporaryAllowanceTimers.Values) timer.Stop();
            _temporaryAllowanceTimers.Clear();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isPaused) return;
            _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerText();
            if (_currentState == PomodoroState.Work)
            {
                _pointsAccumules++;
                UpdatePointsText();
            }
            if (_remaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                if (_currentState == PomodoroState.Work)
                {
                    if (_currentCycle >= _totalCycles) FinishSession();
                    else StartBreakSession();
                }
                else if (_currentState == PomodoroState.ShortBreak)
                {
                    _currentCycle++;
                    StartWorkSession();
                }
            }
        }

        private void UpdateTimerText()
        {
            TimerText.Text = _remaining.ToString(@"mm\:ss");
            string stateInfo = "";
            switch (_currentState)
            {
                case PomodoroState.Work:
                    stateInfo = $"Cycle {_currentCycle}/{_totalCycles} - Travail";
                    break;
                case PomodoroState.ShortBreak:
                    stateInfo = "Petite pause";
                    break;
            }
            if (_isPaused) stateInfo += " (En pause)";
            StateText.Text = stateInfo;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == PomodoroState.Finished) return;
            _isPaused = !_isPaused;
            if (_isPaused)
            {
                _timer.Stop();
                PauseButton.Content = "⏯️ Reprendre";
            }
            else
            {
                _timer.Start();
                PauseButton.Content = "⏯️ Pause";
            }
            UpdateTimerText();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            SavePoints();
            _blocker.SetActive(false);
            foreach (var timer in _temporaryAllowanceTimers.Values) timer.Stop();
            _temporaryAllowanceTimers.Clear();
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    NavigationService?.Navigate(new MenuPage());
                });
            });
        }

        private void UpdatePointsText()
        {
            _pointsText.Text = $"{_pointsAccumules} points";
        }

        private void SavePoints()
        {
            Properties.Settings.Default.Points += _pointsAccumules;
            Properties.Settings.Default.Save();
            string email = Properties.Settings.Default.UserEmail;
            if (!string.IsNullOrWhiteSpace(email))
            {
                UserManager.SavePoints(email, Properties.Settings.Default.Points);
            }
            _pointsAccumules = 0;
            UpdatePointsText();
        }
        #endregion
    }
}