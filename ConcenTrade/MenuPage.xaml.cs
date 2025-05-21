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

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Paramètres ouverts !");
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Voici les statistiques de vos sessions  !");
        }

        private void StartSession_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Décompte session X temps");
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




        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}