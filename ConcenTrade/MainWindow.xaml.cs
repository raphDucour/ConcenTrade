using System.Windows;

namespace Concentrade
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // (optionnel) Pour vérifier la résolution détectée
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            // Si tu veux utiliser ces valeurs plus tard, tu peux les stocker
            // MessageBox.Show($"Résolution détectée : {screenWidth} x {screenHeight}");
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text;
            if (!string.IsNullOrWhiteSpace(name))
            {
                GreetingText.Text = $"Bonjour {name} !";
            }
            else
            {
                GreetingText.Text = "Bonjour !";
            }
        }

        private void NameInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
