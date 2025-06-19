using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private StackPanel _cyclesIndicatorPanel;
        private Storyboard _pulseAnimation;
        private Grid _progressBarTemplateRoot;

        private PomodoroState _currentState;
        private int _totalCycles;
        private int _currentCycle;
        private readonly TimeSpan _workDuration = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _breakDuration = TimeSpan.FromMinutes(1);
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

            InitializeCycleIndicators();

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
            InitializePulseAnimation();
        }

        #region Animation de Pulsation et Particules

        private void InitializePulseAnimation()
        {
            CircularProgressBar.ApplyTemplate();
            _progressBarTemplateRoot = CircularProgressBar.Template.FindName("TemplateRoot", CircularProgressBar) as Grid;
            if (_progressBarTemplateRoot != null && _progressBarTemplateRoot.Resources["PulseAnimation"] is Storyboard storyboard)
            {
                _pulseAnimation = storyboard;
            }
        }

        private void StartPulsing()
        {
            if (_progressBarTemplateRoot != null)
            {
                _pulseAnimation?.Begin(_progressBarTemplateRoot, true);
            }
        }

        private void StopPulsing()
        {
            if (_progressBarTemplateRoot != null)
            {
                _pulseAnimation?.Stop(_progressBarTemplateRoot);
            }
        }

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
            var transform = particle.RenderTransform as TranslateTransform;
            if (transform == null) return;

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

        /// <summary>
        /// NOUVEAU : Déclenche l'animation de convergence et d'illumination des particules.
        /// </summary>
        private void TriggerEndOfCycleParticleAnimation()
        {
            foreach (Ellipse particle in ParticleCanvas.Children.OfType<Ellipse>())
            {
                AnimateParticleToEndOfCycle(particle);
            }
        }

        /// <summary>
        /// NOUVEAU : Anime une seule particule vers le centre, puis la disperse.
        /// </summary>
        private void AnimateParticleToEndOfCycle(Ellipse particle)
        {
            var transform = particle.RenderTransform as TranslateTransform;
            if (transform == null) return;

            // Arrête l'animation de dérive actuelle
            transform.BeginAnimation(TranslateTransform.XProperty, null);
            transform.BeginAnimation(TranslateTransform.YProperty, null);

            Point center = new Point(ParticleCanvas.ActualWidth / 2, ParticleCanvas.ActualHeight / 2);

            var storyboard = new Storyboard();

            // 1. Animation de convergence vers le centre
            var convergeX = new DoubleAnimation(center.X - (particle.Width / 2), TimeSpan.FromSeconds(1.5)) { EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut } };
            var convergeY = new DoubleAnimation(center.Y - (particle.Height / 2), TimeSpan.FromSeconds(1.5)) { EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut } };

            Storyboard.SetTarget(convergeX, particle);
            Storyboard.SetTargetProperty(convergeX, new PropertyPath("RenderTransform.(TranslateTransform.X)"));
            storyboard.Children.Add(convergeX);

            Storyboard.SetTarget(convergeY, particle);
            Storyboard.SetTargetProperty(convergeY, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));
            storyboard.Children.Add(convergeY);

            // 2. Animation d'illumination (opacité)
            var illuminate = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.75)) { AutoReverse = true, EasingFunction = new QuarticEase() };
            Storyboard.SetTarget(illuminate, particle);
            Storyboard.SetTargetProperty(illuminate, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(illuminate);

            // 3. À la fin, disperse la particule et redémarre l'animation normale
            storyboard.Completed += (s, e) => {
                AnimateParticle(particle);
            };

            storyboard.Begin();
        }

        #endregion

        #region Logique du Timer Pomodoro

        private void InitializeCycleIndicators()
        {
            _cyclesIndicatorPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 20)
            };

            DisplayPanel.Children.Insert(0, _cyclesIndicatorPanel);

            for (int i = 0; i < _totalCycles; i++)
            {
                var circle = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
                    Margin = new Thickness(5)
                };
                _cyclesIndicatorPanel.Children.Add(circle);
            }
        }

        private void UpdateCycleIndicators()
        {
            for (int i = 0; i < _totalCycles; i++)
            {
                if (_cyclesIndicatorPanel.Children[i] is Ellipse circle)
                {
                    if (i < _currentCycle - 1)
                    {
                        circle.Fill = new SolidColorBrush(Colors.White);
                    }
                    else if (i == _currentCycle - 1 && _currentState != PomodoroState.Finished)
                    {
                        circle.Fill = new SolidColorBrush(Color.FromRgb(0, 224, 255));
                    }
                    else
                    {
                        circle.Fill = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
                    }
                }
            }
        }

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
            UpdateTimerDisplay(true);
            UpdateCycleIndicators();
            _timer.Start();
            StartPulsing();
        }

        private void StartBreakSession()
        {
            StopPulsing();
            _currentState = PomodoroState.ShortBreak;
            _duration = _breakDuration;
            _remaining = _duration;
            _isPaused = false;
            PauseButton.Content = "⏯️ Pause";
            _blocker.SetActive(false);
            UpdateTimerDisplay(true);
            UpdateCycleIndicators();
            _timer.Start();
        }

        private void FinishSession()
        {
            StopPulsing();
            _timer.Stop();
            _currentState = PomodoroState.Finished;
            StateText.Text = "Félicitations !";
            TimerText.Text = "�";
            UpdateCycleIndicators();
            SavePoints();
            _blocker.SetActive(false);
        }

        // MISE À JOUR : La méthode est maintenant asynchrone pour permettre une pause
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isPaused) return;

            _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerDisplay();

            if (_currentState == PomodoroState.Work)
            {
                _pointsAccumules++;
                UpdatePointsText();
            }

            if (_remaining.TotalSeconds <= 0)
            {
                _timer.Stop();

                // NOUVEAU : Déclenche l'animation des particules et attend un peu
                TriggerEndOfCycleParticleAnimation();
                await Task.Delay(1600); // Laisse le temps à l'animation de se jouer

                if (_currentState == PomodoroState.Work)
                {
                    if (_currentCycle >= _totalCycles)
                    {
                        FinishSession();
                    }
                    else
                    {
                        StartBreakSession();
                    }
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
                AnimateProgressBar(newValue, isInitialSet);
            }

            string stateInfo = "";
            switch (_currentState)
            {
                case PomodoroState.Work:
                    stateInfo = "Travail";
                    break;
                case PomodoroState.ShortBreak:
                    stateInfo = "Petite pause";
                    break;
            }
            if (_isPaused)
            {
                stateInfo += " (En pause)";
            }

            StateText.Text = stateInfo;
        }

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
                Duration = TimeSpan.FromMilliseconds(950),
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
                StopPulsing();
                PauseButton.Content = "▶️ Reprendre";
            }
            else
            {
                _timer.Start();
                if (_currentState == PomodoroState.Work)
                {
                    StartPulsing();
                }
                PauseButton.Content = "⏯️ Pause";
            }
            UpdateTimerDisplay();
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopPulsing();
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