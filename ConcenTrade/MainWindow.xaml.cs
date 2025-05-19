using System.Windows;

namespace Concentrade
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Détection résolution (optionnel)
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            // Chargement du prénom sauvegardé
            string savedName = Properties.Settings.Default.UserName;
            if (!string.IsNullOrWhiteSpace(savedName))
            {
                GreetingText.Text = $"Rebonjour {savedName} !";
                NameInput.Text = savedName;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text;

            if (!string.IsNullOrWhiteSpace(name))
            {
                // Sauvegarde du prénom
                Properties.Settings.Default.UserName = name;
                Properties.Settings.Default.Save();

                GreetingText.Text = $"Bonjour {name} !";
            }
            else
            {
                GreetingText.Text = "Bonjour !";
            }
        }

        private void NameInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            GreetingText.Text = ""; // Nettoie l’ancien message pendant la saisie
        }
    }
}
