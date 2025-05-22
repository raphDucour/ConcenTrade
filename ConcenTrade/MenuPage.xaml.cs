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
            this.NavigationService?.Navigate(new QuestionPrenom());
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
            MessageBox.Show("Données réinitialisées.");
        }
    }
}
