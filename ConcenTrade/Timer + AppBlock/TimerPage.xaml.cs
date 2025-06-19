using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Concentrade
{
    public partial class TimerPage : Page
    {
        private enum PomodoroState { Work, ShortBreak, Finished }

        private DispatcherTimer _timer;
        private TimeSpan _remaining;
        private TimeSpan _duration;
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
        private Random _random = new Random();

        public TimerPage(int cycles)
        {
            InitializeComponent();
            _totalCycles = cycles;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;

            _pointsText = new TextBlock
            {
                FontSize = 20,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 20, 20, 0)
            };
            UpdatePointsText();
            MainGrid.Children.Add(_pointsText);

            _blocker = ((App)Application.Current).AppBlocker;

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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                CreateAndAnimateParticles(15);
            }
        }

        #region Animation des Particules
        private void CreateAndAnimateParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Ellipse particle = new Ellipse
                {
                    Fill = new SolidColorBrush(Colors.White),
                    Effect = new System.Windows.Media.Effects.BlurEffect()
                };

                double size = _random.Next(5, 40);
                particle.Width = size;
                particle.Height = size;
                particle.Opacity = _random.NextDouble() * 0.4 + 0.1;
                ((System.Windows.Media.Effects.BlurEffect)particle.Effect).Radius = _random.Next(5, 15);

                particle.RenderTransform = new TranslateTransform(_random.Next(0, (int)this.ActualWidth), _random.Next(0, (int)this.ActualHeight));

                ParticleCanvas.Children.Add(particle);
                AnimateParticle(particle);
            }
        }

        private void AnimateParticle(Ellipse particle)
        {
            var transform = (TranslateTransform)particle.RenderTransform;

            double endX = _random.NextDouble() > 0.5 ? this.ActualWidth + 100 : -100;
            double endY = _random.Next(0, (int)this.ActualHeight);

            var animX = new DoubleAnimation
            {
                To = endX,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)),
            };

            var animY = new DoubleAnimation
            {
                To = endY,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)),
            };

            animX.Completed += (s, e) =>
            {
                if (this.ActualWidth > 0 && this.ActualHeight > 0)
                {
                    transform.X = _random.NextDouble() > 0.5 ? -50 : this.ActualWidth + 50;
                    transform.Y = _random.Next(0, (int)this.ActualHeight);
                    AnimateParticle(particle);
                }
            };

            transform.BeginAnimation(TranslateTransform.XProperty, animX);
            transform.BeginAnimation(TranslateTransform.YProperty, animY);
        }
        #endregion

        #region Logique du Timer Pomodoro
        private void StartPomodoro()
        {
            _currentCycle = 1;
            StartWorkSession();
        }

        private void StartWorkSession()
        {
            _currentState = PomodoroState.Work;
            _duration = _workDuration;
            _remaining = _duration;
            _isPaused = false;
            PauseButton.Content = "⏯️ Pause";
            _blocker.SetActive(true);
            UpdateTimerDisplay(true); // true pour une mise à jour instantanée au début
            _timer.Start();
        }

        private void StartBreakSession()
        {
            _currentState = PomodoroState.ShortBreak;
            _duration = _breakDuration;
            _remaining = _duration;
            _isPaused = false;
            PauseButton.Content = "⏯️ Pause";
            _blocker.SetActive(false);
            UpdateTimerDisplay(true); // true pour une mise à jour instantanée au début
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
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isPaused) return;

            _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerDisplay(); // La mise à jour est maintenant animée

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

        private void UpdateTimerDisplay(bool isInitialSet = false)
        {
            TimerText.Text = _remaining.ToString(@"mm\:ss");

            if (_duration.TotalSeconds > 0)
            {
                double newValue = (_remaining.TotalSeconds / _duration.TotalSeconds) * 100;
                // Animer la barre de progression au lieu de la définir directement
                AnimateProgressBar(newValue, isInitialSet);
            }

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

        /// <summary>
        /// Anime la barre de progression vers une nouvelle valeur.
        /// </summary>
        /// <param name="newValue">La nouvelle valeur (0-100).</param>
        /// <param name="isInitialSet">Si vrai, la valeur est définie instantanément sans animation.</param>
        private void AnimateProgressBar(double newValue, bool isInitialSet = false)
        {
            if (isInitialSet)
            {
                CircularProgressBar.Value = newValue;
                return;
            }

            var animation = new DoubleAnimation
            {
                To = newValue,
                Duration = TimeSpan.FromMilliseconds(950), // Animation fluide sur presque une seconde
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };
            CircularProgressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == PomodoroState.Finished) return;

            _isPaused = !_isPaused;
            if (_isPaused)
            {
                _timer.Stop();
                PauseButton.Content = "▶️ Reprendre";
            }
            else
            {
                _timer.Start();
                PauseButton.Content = "⏯️ Pause";
            }
            UpdateTimerDisplay();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            SavePoints();
            _blocker.SetActive(false);

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
