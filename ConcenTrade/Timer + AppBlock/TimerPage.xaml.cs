using ConcenTrade;
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
        private TimeSpan _workDuration = TimeSpan.FromMinutes(25);
        private TimeSpan _breakDuration = TimeSpan.FromMinutes(5);
        private Random _random = new Random();
        private List<MediaPlayer> _activeSoundPlayers = new List<MediaPlayer>();
        private DispatcherTimer _distractionPauseTimer;
        private bool _isExtensionPopupShown = false;
        private bool _isFocusMode;

        private enum PomodoroState { Work, ShortBreak, Finished, Idle }

        // Initialise la page timer pour le mode Pomodoro classique
        public TimerPage(int cycles, bool isFocusMode = false)
        {
            InitializeComponent();
            _totalCycles = cycles;
            _isFocusMode = isFocusMode;

            _workDuration = TimeSpan.FromMinutes(25);
            _breakDuration = TimeSpan.FromMinutes(5);

            InitializeTimerPage();
        }

        // Initialise la page timer pour le mode personnalisé
        public TimerPage(TimeSpan workDuration, TimeSpan breakDuration, int cycles, bool isFocusMode)
        {
            InitializeComponent();

            _workDuration = workDuration;
            _breakDuration = breakDuration;
            _totalCycles = cycles;
            _isFocusMode = isFocusMode;

            InitializeTimerPage();
        }

        // Initialise les composants communs de la page timer
        private void InitializeTimerPage()
        {
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

        // Gère l'événement de chargement de la page
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                CreateAndAnimateParticles(15);
            }
            InitializePulseAnimation();
        }

        // Initialise l'animation de pulsation
        private void InitializePulseAnimation()
        {
            CircularProgressBar.ApplyTemplate();
            _progressBarTemplateRoot = CircularProgressBar.Template.FindName("TemplateRoot", CircularProgressBar) as Grid;
            if (_progressBarTemplateRoot != null && _progressBarTemplateRoot.Resources["PulseAnimation"] is Storyboard storyboard)
            {
                _pulseAnimation = storyboard;
            }
        }

        // Démarre l'animation de pulsation
        private void StartPulsing()
        {
            if (_progressBarTemplateRoot != null)
            {
                _pulseAnimation?.Begin(_progressBarTemplateRoot, true);
            }
        }

        // Arrête l'animation de pulsation
        private void StopPulsing()
        {
            if (_progressBarTemplateRoot != null)
            {
                _pulseAnimation?.Stop(_progressBarTemplateRoot);
            }
        }

        // Crée et anime les particules d'arrière-plan
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

        // Anime une particule individuelle
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

        // Déclenche l'animation des particules à la fin d'un cycle
        private void TriggerEndOfCycleParticleAnimation()
        {
            foreach (Ellipse particle in ParticleCanvas.Children.OfType<Ellipse>())
            {
                AnimateParticleToEndOfCycle(particle);
            }
        }

        // Anime une particule vers le centre à la fin d'un cycle
        private void AnimateParticleToEndOfCycle(Ellipse particle)
        {
            var transform = particle.RenderTransform as TranslateTransform;
            if (transform == null) return;

            transform.BeginAnimation(TranslateTransform.XProperty, null);
            transform.BeginAnimation(TranslateTransform.YProperty, null);

            Point center = new Point(ParticleCanvas.ActualWidth / 2, ParticleCanvas.ActualHeight / 2);

            var storyboard = new Storyboard();

            var convergeX = new DoubleAnimation(center.X - (particle.Width / 2), TimeSpan.FromSeconds(1.5)) { EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut } };
            var convergeY = new DoubleAnimation(center.Y - (particle.Height / 2), TimeSpan.FromSeconds(1.5)) { EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut } };

            Storyboard.SetTarget(convergeX, particle);
            Storyboard.SetTargetProperty(convergeX, new PropertyPath("RenderTransform.(TranslateTransform.X)"));
            storyboard.Children.Add(convergeX);

            Storyboard.SetTarget(convergeY, particle);
            Storyboard.SetTargetProperty(convergeY, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));
            storyboard.Children.Add(convergeY);

            var illuminate = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.75)) { AutoReverse = true, EasingFunction = new QuarticEase() };
            Storyboard.SetTarget(illuminate, particle);
            Storyboard.SetTargetProperty(illuminate, new PropertyPath(UIElement.OpacityProperty));
            storyboard.Children.Add(illuminate);

            storyboard.Completed += (s, e) => {
                AnimateParticle(particle);
            };

            storyboard.Begin();
        }

        // Initialise les indicateurs de cycles
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

        // Met à jour l'affichage des indicateurs de cycles
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

        // Démarre la session Pomodoro
        private void StartPomodoro()
        {
            _currentCycle = 1;
            StartWorkSession();
        }

        // Démarre une session de travail
        private void StartWorkSession()
        {
            _currentState = PomodoroState.Work;
            _duration = _workDuration;
            _remaining = _duration;
            _isPaused = false;
            PauseButton.Content = "⏯️ Pause";
            _blocker.SetActive(true);
            _blocker.SetActive(true, _isFocusMode);
            UpdateTimerDisplay(true);
            UpdateCycleIndicators();
            _timer.Start();
            StartPulsing();
        }

        // Démarre une session de pause
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

        // Termine la session complète
        private void FinishSession()
        {
            StopPulsing();
            _timer.Stop();
            _currentState = PomodoroState.Finished;
            StateText.Text = "Félicitations !";
            TimerText.Text = "🎉";
            UpdateCycleIndicators();
            SavePoints();
            _blocker.SetActive(false);
        }

        // Gère le tick du timer principal
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isPaused) return;

            _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerDisplay();

            if (_currentState == PomodoroState.Work)
            {
                if (_duration.TotalSeconds - _remaining.TotalSeconds > 0 && 
                    (_duration.TotalSeconds - _remaining.TotalSeconds) % 299 == 0)
                {
                    _pointsAccumules++;
                    UpdatePointsText();
                }
            }

            if (_remaining.TotalSeconds <= 0)
            {
                _timer.Stop();

                TriggerEndOfCycleParticleAnimation();
                await Task.Delay(1600);

                if (_currentState == PomodoroState.Work)
                {
                    PlaySound("Images/mp3/fin_travail.mp3");

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
                    PlaySound("Images/mp3/debut_travail.mp3");

                    _currentCycle++;
                    StartWorkSession();
                }
            }
        }

        // Joue un son à partir d'un fichier
        private void PlaySound(string uri)
        {
            try
            {
                var player = new MediaPlayer();
                _activeSoundPlayers.Add(player);

                player.MediaEnded += (sender, e) =>
                {
                    if (sender is MediaPlayer mp)
                    {
                        mp.Close();
                        _activeSoundPlayers.Remove(mp);
                    }
                };

                player.MediaFailed += (sender, e) =>
                {
                    MessageBox.Show($"Erreur Média : {e.ErrorException.Message}");
                    if (sender is MediaPlayer mp)
                    {
                        _activeSoundPlayers.Remove(mp);
                    }
                };

                player.Open(new Uri(uri, UriKind.Relative));
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la lecture du son : {ex.Message}", "Erreur Audio");
            }
        }

        // Met à jour l'affichage du timer
        private void UpdateTimerDisplay(bool isInitialSet = false)
        {
            TimerText.Text = string.Format("{0:00}:{1:00}", (int)_remaining.TotalMinutes, _remaining.Seconds);

            if (_duration.TotalSeconds > 0)
            {
                double newValue = (_remaining.TotalSeconds / _duration.TotalSeconds) * 100;
                AnimateProgressBar(newValue, isInitialSet);
            }

            string stateInfo = "";
            switch (_currentState)
            {
                case PomodoroState.Work:
                    stateInfo = $"Cycle {_currentCycle}/{_totalCycles} - Travail";
                    break;
                case PomodoroState.ShortBreak:
                    stateInfo = $"Cycle {_currentCycle}/{_totalCycles} - Petite pause";
                    break;
                case PomodoroState.Finished:
                    stateInfo = "Félicitations !";
                    break;
                default:
                    stateInfo = "";
                    break;
            }
            if (_isPaused && _currentState != PomodoroState.Finished)
            {
                stateInfo += " (En pause)";
            }

            StateText.Text = stateInfo;
        }

        // Anime la barre de progression
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

        // Gère le clic sur le bouton pause/reprendre
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

        // Gère le clic sur le bouton stop
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            
            StopPulsing();
            _timer.Stop();
            _distractionPauseTimer?.Stop();
            SavePoints();
            _blocker.SetActive(false); 
            
            UserManager.PushIntoBDD_FireAndForget();

            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new MenuPage());
            }
        }

        // Met à jour l'affichage des points
        private void UpdatePointsText()
        {
            _pointsText.Text = $"{_pointsAccumules} points";
        }

        // Sauvegarde les points accumulés
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

        // Gère l'autorisation temporaire d'une application
        private void Blocker_OnTemporaryAllowance(object? sender, TemporaryAllowanceEventArgs e)
        {
            if (_currentState != PomodoroState.Work || _isPaused) return;

            _timer.Stop();
            _isPaused = true;
            PauseButton.Content = "▶️ Reprendre";
            StateText.Text = $"En pause - {e.ProcessName} autorisé";

            StartDistractionTimer(e.ProcessName, e.Duration, isExtension: false);
        }

        // Démarre le timer de distraction
        private void StartDistractionTimer(string processName, TimeSpan duration, bool isExtension)
        {
            _distractionPauseTimer?.Stop();

            _distractionPauseTimer = new DispatcherTimer { Interval = duration };
            _distractionPauseTimer.Tag = new Tuple<string, bool>(processName, isExtension);

            _distractionPauseTimer.Tick += DistractionTimer_Finished;
            _distractionPauseTimer.Start();
        }

        // Gère la fin du timer de distraction
        private async void DistractionTimer_Finished(object? sender, EventArgs e)
        {
            _distractionPauseTimer?.Stop();

            if (sender is not DispatcherTimer timer || timer.Tag is not Tuple<string, bool> tag)
            {
                ResumeWorkTimer();
                return;
            }

            var processName = tag.Item1;
            var wasAnExtension = tag.Item2;

            if (wasAnExtension)
            {
                _blocker.KillApplication(processName);
                ResumeWorkTimer();
            }
            else
            {
                if (_isExtensionPopupShown)
                {
                    return;
                }

                try
                {
                    _isExtensionPopupShown = true;

                    await Task.Yield();
                    _blocker.MinimizeProcess(processName);

                    var extensionWindow = new ExtensionChoiceWindow(processName);
                    extensionWindow.Activate();
                    bool? dialogResult = extensionWindow.ShowDialog();

                    if (dialogResult == true)
                    {
                        StateText.Text = $"Pause prolongée pour {processName}";
                        StartDistractionTimer(processName, extensionWindow.ExtensionDuration, isExtension: true);
                    }
                    else
                    {
                        _blocker.KillApplication(processName);
                        ResumeWorkTimer();
                    }
                }
                finally
                {
                    _isExtensionPopupShown = false;
                }
            }
        }

        // Reprend le timer de travail
        private void ResumeWorkTimer()
        {
            if (_currentState == PomodoroState.Work)
            {
                _timer.Start();
                _isPaused = false;
                PauseButton.Content = "⏯️ Pause";
                UpdateTimerDisplay();
            }
        }
    }
}