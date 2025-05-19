using System.Windows;

namespace Concentrade
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Ferme la fenêtre menu
        }
    }
}
