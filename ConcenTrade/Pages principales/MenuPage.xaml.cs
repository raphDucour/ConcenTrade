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

            CustomTimeSlider_ValueChanged(null, null);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsTutorialDone = true;
            Properties.Settings.Default.Save();
            
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
            return (int)Math.Round(position);
        }

        private void DureeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderLabel == null) return;

            int cycles = (int)Math.Round(DureeSlider.Value);
            int dureeTravail = cycles * 25;
            int dureePause = (cycles > 1) ? (cycles - 1) * 5 : 0;
            int dureeTotale = dureeTravail + dureePause;

            SliderLabel.Text = $"Cycles : {cycles} (Total : {dureeTotale} min)";
        }

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
            CycleCountLabel.Text = $"Nombre de cycles : {(int)CycleCountSlider.Value}";
        }

        public void StartSession_Click(object sender, RoutedEventArgs e)
        {
            bool isFocusMode = FocusModeToggle.IsChecked == true;

            if (PomodoroModeButton.IsChecked == true)
            {
                int cycles = ConvertirPositionEnCycles(DureeSlider.Value);

                this.NavigationService?.Navigate(new TimerPage(TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5), cycles, isFocusMode));
            }
            else
            {
                TimeSpan workDuration = TimeSpan.FromMinutes((int)WorkTimeSlider.Value);
                TimeSpan breakDuration = TimeSpan.FromMinutes((int)BreakTimeSlider.Value);
                int customCycles = (int)CycleCountSlider.Value;

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
            this.NavigationService?.Navigate(new Pages_principales.SessionHistoryPage());
        }

        private void Tutorial_Click(object sender, RoutedEventArgs e)
        {
            StartTutorial();
        }

        private void StartTutorial()
        {
            var targets = new UIElement[] {
                null,
                PomodoroModeButton,
                CustomModeButton,
                FocusModeToggle,
                StartSessionButton,
                SettingsButton,
                CollectionButton
            };
            var texts = new string[] {
                "Avant de commencer, nous allons vous présenter les différents boutons et fonctionnalités de l'application.",
                "Mode Pomodoro : 25 minutes de travail, 5 minutes de pause, répétés sur plusieurs cycles pour maximiser ta concentration !\n\nAstuce : règle le nombre de cycles juste en dessous pour choisir combien de fois tu veux enchaîner 25 min de travail puis 5 min de pause dans ta session.",
                "Mode Temps personnalisé : choisis toi-même la durée de travail, de pause et le nombre de cycles selon tes besoins.",
                "Active le Mode Focus pour bloquer les applications distrayantes.\n\n⚠️ En mode normal, si tu ouvres une application bloquée, tu peux choisir de la fermer ou de continuer temporairement.\nEn mode Focus, tu n'auras qu'un seul choix : l'application sera automatiquement bloquée et tu ne pourras pas l'utiliser tant que la session est en cours.",
                "Clique ici pour démarrer une session de concentration.",
                "Accède aux paramètres de l'application. C'est ici que tu peux modifier ton prénom, ta date de naissance, et ajouter ou supprimer des applications distrayantes à bloquer.",
                "Ici, tu peux accéder à ta collection de cartes et acheter des caisses pour débloquer de nouvelles cartes et agrandir ta collection."
            };
            TutorialOverlayControl.StartTutorial(targets, texts);
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

        private void Collection_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new Pages_principales.CollectionPage());
        }
    }
}