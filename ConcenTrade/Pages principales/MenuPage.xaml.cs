using ConcenTrade;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Concentrade
{
    public partial class MenuPage : Page, INotifyPropertyChanged
    {
        private readonly int[] _cyclesPossibles = new[] { 1, 2, 3, 4 };
        private readonly int[] _positionsSlider = new[] { 0, 33, 67, 100 };
        private int _points;
        private Random _random = new Random();

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Points
        {
            get => _points;
            set
            {
                if (_points != value)
                {
                    _points = value;
                    Properties.Settings.Default.Points = value;
                    Properties.Settings.Default.Save();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Points)));
                }
            }
        }

        public MenuPage()
        {
            InitializeComponent();
            DataContext = this;
            Points = Properties.Settings.Default.Points;
            DureeSlider.Value = 50;

            // ✅ LIGNE MODIFIÉE : On initialise les labels des sliders personnalisés au démarrage
            CustomTimeSlider_ValueChanged(null, null);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Properties.Settings.Default.IsTutorialDone)
            {
                var targets = new UIElement[] { null, StartSessionButton, FocusModeToggle, SettingsButton, CollectionButton };
                var texts = new string[] {
                    "Avant de commencer, nous allons vous présenter les différents boutons et fonctionnalités de l'application.",
                    "Clique ici pour démarrer une session de concentration.",
                    "Active le Mode Focus pour bloquer les applications distrayantes.",
                    "Accède aux paramètres de l'application.",
                    "Consulte ta collection de cartes ici."
                };
                TutorialOverlayControl.StartTutorial(targets, texts);
                //Properties.Settings.Default.IsTutorialDone = true;      A remettre dans le code si on veut pas que le tuto se lance a chaque fois
                Properties.Settings.Default.Save();
            }
            CreateAndAnimateParticles(10);
        }

        private void CreateAndAnimateParticles(int count)
        {
            if (ActualWidth == 0 || ActualHeight == 0) return;

            for (int i = 0; i < count; i++)
            {
                Ellipse particle = new Ellipse
                {
                    Fill = new SolidColorBrush(Colors.White),
                    Effect = new BlurEffect()
                };

                double size = _random.Next(5, 40);
                particle.Width = size;
                particle.Height = size;
                particle.Opacity = _random.NextDouble() * 0.4 + 0.1;
                ((BlurEffect)particle.Effect).Radius = _random.Next(5, 15);

                particle.RenderTransform = new TranslateTransform(_random.Next(0, (int)ActualWidth), _random.Next(0, (int)ActualHeight));

                ParticleCanvas.Children.Add(particle);

                AnimateParticle(particle);
            }
        }

        private void AnimateParticle(Ellipse particle)
        {
            var transform = (TranslateTransform)particle.RenderTransform;

            double endX = _random.NextDouble() > 0.5 ? ActualWidth + 100 : -100;
            double endY = _random.Next(0, (int)ActualHeight);

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
                if (ActualWidth > 0 && ActualHeight > 0)
                {
                    transform.X = _random.NextDouble() > 0.5 ? -50 : ActualWidth + 50;
                    transform.Y = _random.Next(0, (int)ActualHeight);
                    AnimateParticle(particle);
                }
            };

            transform.BeginAnimation(TranslateTransform.XProperty, animX);
            transform.BeginAnimation(TranslateTransform.YProperty, animY);
        }

        private int ConvertirPositionEnCycles(double position)
        {
            int index = _positionsSlider.Select((p, i) => new { Index = i, Distance = Math.Abs(p - position) })
                                         .OrderBy(p => p.Distance)
                                         .First().Index;
            return _cyclesPossibles[index];
        }

        private void DureeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderLabel == null) return;

            double valeurActuelle = e.NewValue;
            int positionPlusProche = _positionsSlider.OrderBy(p => Math.Abs(p - valeurActuelle)).First();

            if (Math.Abs(DureeSlider.Value - positionPlusProche) > 5)
            {
                DureeSlider.Value = positionPlusProche;
            }

            int cycles = ConvertirPositionEnCycles(DureeSlider.Value);
            int dureeTravail = cycles * 25;
            int dureePause = (cycles > 1) ? (cycles - 1) * 5 : 0;
            int dureeTotale = dureeTravail + dureePause;

            SliderLabel.Text = $"Cycles : {cycles} (Total : {dureeTotale} min)";
        }

        // ✅ NOUVELLE MÉTHODE
        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (PomodoroPanel == null || CustomTimePanel == null) return;

            if (PomodoroModeButton.IsChecked == true)
            {
                PomodoroPanel.Visibility = Visibility.Visible;
                CustomTimePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                PomodoroPanel.Visibility = Visibility.Collapsed;
                CustomTimePanel.Visibility = Visibility.Visible;
            }
        }

        private void CustomTimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (WorkTimeLabel == null || BreakTimeLabel == null || CycleCountLabel == null) return;

            WorkTimeLabel.Text = $"Temps de travail : {(int)WorkTimeSlider.Value} minutes";
            BreakTimeLabel.Text = $"Temps de pause : {(int)BreakTimeSlider.Value} minutes";
            // ✅ Ligne ajoutée
            CycleCountLabel.Text = $"Nombre de cycles : {(int)CycleCountSlider.Value}";
        }

        // ✅ MÉTHODE MODIFIÉE
        private void StartSession_Click(object sender, RoutedEventArgs e)
        {
            // On récupère l'état du Mode Focus au moment du clic
            bool isFocusMode = FocusModeToggle.IsChecked == true;

            // On vérifie quel mode de timer est sélectionné (Pomodoro ou Personnalisé)
            if (PomodoroModeButton.IsChecked == true)
            {
                // Logique pour le mode Pomodoro
                int cycles = ConvertirPositionEnCycles(DureeSlider.Value);

                // On navigue vers la TimerPage en passant les durées standards de Pomodoro (25/5)
                // et l'état du Mode Focus.
                this.NavigationService?.Navigate(new TimerPage(TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5), cycles, isFocusMode));
            }
            else
            {
                // Logique pour le mode Personnalisé
                TimeSpan workDuration = TimeSpan.FromMinutes((int)WorkTimeSlider.Value);
                TimeSpan breakDuration = TimeSpan.FromMinutes((int)BreakTimeSlider.Value);
                int customCycles = (int)CycleCountSlider.Value;

                // On navigue vers la TimerPage en passant les durées personnalisées
                // et l'état du Mode Focus.
                this.NavigationService?.Navigate(new TimerPage(workDuration, breakDuration, customCycles, isFocusMode));
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = false;
            this.NavigationService?.Navigate(new SettingsPage());
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new Pages_principales.CollectionPage());
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ResetData_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = false;
            MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir réinitialiser vos données ? Cette action est irréversible.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
                if (Application.Current.MainWindow is MainWindow mainWindow) { mainWindow.NavigateTo(new LoginPage()); }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = false;
            Properties.Settings.Default.UserEmail = "";
            Properties.Settings.Default.Save();
            this.NavigationService?.Navigate(new LoginPage());
        }
    }
}