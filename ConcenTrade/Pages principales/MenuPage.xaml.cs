using ConcenTrade;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Concentrade
{
    public partial class MenuPage : Page, INotifyPropertyChanged
    {
        private readonly int[] _dureesPossibles = new[] { 5, 15, 25, 45, 60 };
        // Positions visuelles sur le slider (0-100)
        private readonly int[] _positionsSlider = new[] { 0, 25, 50, 75, 100 };
        private int _points;

        public event PropertyChangedEventHandler PropertyChanged;

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
            DureeSlider.Value = 50; // Position par défaut (25 minutes)
        }

        private int ConvertirPositionEnDuree(double position)
        {
            // Trouver l'index de la position la plus proche
            int index = 0;
            double minDistance = double.MaxValue;
            
            for (int i = 0; i < _positionsSlider.Length; i++)
            {
                double distance = Math.Abs(_positionsSlider[i] - position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                }
            }
            
            return _dureesPossibles[index];
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

        private void DureeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderLabel == null) return;

            // Trouver la position la plus proche
            double valeurActuelle = e.NewValue;
            int positionPlusProche = _positionsSlider.OrderBy(p => Math.Abs(p - valeurActuelle)).First();
            
            // Mettre à jour le slider avec la position exacte
            if (Math.Abs(valeurActuelle - positionPlusProche) > 0.1)
            {
                DureeSlider.Value = positionPlusProche;
            }
            
            // Convertir la position en durée et mettre à jour le label
            int duree = ConvertirPositionEnDuree(positionPlusProche);
            SliderLabel.Text = $"Durée : {duree} min";
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
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
            MessageBoxResult result = MessageBox.Show(
                "Êtes-vous sûr de vouloir réinitialiser vos données ? Cette action est irréversible.",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // ✅ Réinitialisation des données
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();

                // 🔁 Retour à la page Questionnaire
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.NavigateTo(new QuestionPrenom());
                }
            }
            else
            {
                // ❌ L'utilisateur a cliqué sur "Non"
                MessageBox.Show("Réinitialisation annulée.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // 🔒 Redirection vers la page de connexion
            this.NavigationService?.Navigate(new LoginPage());
        }
    }
}
