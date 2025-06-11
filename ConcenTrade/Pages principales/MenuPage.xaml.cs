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
        private readonly int[] _dureesPossibles = new[] { 5, 15, 25, 45, 60 };
        private readonly int[] _positionsSlider = new[] { 0, 25, 50, 75, 100 };
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
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Lancement de la nouvelle animation de particules
            CreateAndAnimateParticles(25);
        }

        private void CreateAndAnimateParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // 1. Créer une particule (un petit cercle)
                Ellipse particle = new Ellipse
                {
                    Fill = new SolidColorBrush(Colors.White),
                    Effect = new BlurEffect()
                };

                // 2. Donner des propriétés aléatoires pour un effet de profondeur
                double size = _random.Next(5, 40);
                particle.Width = size;
                particle.Height = size;
                particle.Opacity = _random.NextDouble() * 0.4 + 0.1; // Opacité entre 0.1 et 0.5
                ((BlurEffect)particle.Effect).Radius = _random.Next(5, 15);

                // 3. Positionner la particule aléatoirement
                particle.RenderTransform = new TranslateTransform(_random.Next(0, (int)ActualWidth), _random.Next(0, (int)ActualHeight));

                // 4. Ajouter la particule à la zone de dessin
                ParticleCanvas.Children.Add(particle);

                // 5. Animer la particule
                AnimateParticle(particle);
            }
        }

        private void AnimateParticle(Ellipse particle)
        {
            var transform = (TranslateTransform)particle.RenderTransform;

            // Déterminer une destination aléatoire hors de l'écran
            double endX = _random.NextDouble() > 0.5 ? ActualWidth + 100 : -100;
            double endY = _random.Next(0, (int)ActualHeight);

            // Animer la position X
            var animX = new DoubleAnimation
            {
                To = endX,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)), // Durée lente et aléatoire
            };

            // Animer la position Y
            var animY = new DoubleAnimation
            {
                To = endY,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)),
            };

            // Quand l'animation est finie, on la relance
            animX.Completed += (s, e) =>
            {
                // Réinitialiser la position à un bord de l'écran
                transform.X = _random.NextDouble() > 0.5 ? -50 : ActualWidth + 50;
                transform.Y = _random.Next(0, (int)ActualHeight);
                AnimateParticle(particle); // Relancer l'animation en boucle
            };

            transform.BeginAnimation(TranslateTransform.XProperty, animX);
            transform.BeginAnimation(TranslateTransform.YProperty, animY);
        }


        // --- Le reste du code reste identique ---

        private int ConvertirPositionEnDuree(double position)
        {
            int index = _positionsSlider.Select((p, i) => new { Index = i, Distance = Math.Abs(p - position) })
                                        .OrderBy(p => p.Distance)
                                        .First().Index;
            return _dureesPossibles[index];
        }

        private void DureeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderLabel == null) return;
            double valeurActuelle = e.NewValue;
            int positionPlusProche = _positionsSlider.OrderBy(p => Math.Abs(p - valeurActuelle)).First();
            if (Math.Abs(DureeSlider.Value - positionPlusProche) > 0.1) { DureeSlider.Value = positionPlusProche; }
            int duree = ConvertirPositionEnDuree(positionPlusProche);
            SliderLabel.Text = $"Durée de la session : {duree} min";
        }

        private void StartSession_Click(object sender, RoutedEventArgs e)
        {
            int duree = ConvertirPositionEnDuree(DureeSlider.Value);
            this.NavigationService?.Navigate(new TimerPage(duree));
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