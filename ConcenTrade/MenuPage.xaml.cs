using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();

            string name = Properties.Settings.Default.UserName;
            GreetingText.Text = !string.IsNullOrWhiteSpace(name)
                ? $"Bonjour {name} !"
                : "Bonjour !";

        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Page des paramètres à venir !");
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Page des statistiques à venir !");
        }
    }
}
