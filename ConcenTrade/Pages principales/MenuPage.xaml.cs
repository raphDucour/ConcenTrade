using ConcenTrade;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    public partial class MenuPage : Page, INotifyPropertyChanged
    {
        private int _points;
        public int Points
        {
            get => _points;
            set
            {
                if (_points != value)
                {
                    _points = value;
                    OnPropertyChanged(nameof(Points));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MenuPage()
        {
            InitializeComponent();
            DataContext = this;
            Points = Properties.Settings.Default.Points;
        }

        private void StartSession_Click(object sender, RoutedEventArgs e)
        {
            int duree = (int)Math.Round(DureeSlider.Value);
            this.NavigationService?.Navigate(new TimerPage(duree));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = !SettingsPopup.IsOpen;
        }

        private void DureeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderLabel != null)
                SliderLabel.Text = $"Durée : {Math.Round(e.NewValue)} min";
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new SettingsPage());
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Statistiques à venir.");
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
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
                Points = 0;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Sauvegarder les points actuels dans le fichier JSON
            string email = Properties.Settings.Default.UserEmail;
            if (!string.IsNullOrWhiteSpace(email))
            {
                UserManager.SavePoints(email, Points);
            }

            // Réinitialiser les paramètres locaux
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            Points = 0;

            // 🔒 Redirection vers la page de connexion
            this.NavigationService?.Navigate(new LoginPage());
        }
    }
}
