using System;
using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void StartSession_Click(object sender, RoutedEventArgs e)
        {
            int duree = (int)Math.Round(DureeSlider.Value);
            this.NavigationService?.Navigate(new TimerPage(duree));
        }


        private void DureeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderLabel != null)
                SliderLabel.Text = $"Durée : {Math.Round(e.NewValue)} min";
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Paramètres à venir.");
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
    }
}
