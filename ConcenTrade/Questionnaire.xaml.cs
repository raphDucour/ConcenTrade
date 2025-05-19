using System.Windows;

namespace Concentrade
{
    public partial class Questionnaire : Window
    {
        public Questionnaire()
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
                // Enregistrement et sauvegarde
                Properties.Settings.Default.UserName = name;
                Properties.Settings.Default.Save();

                // Affiche un message de bienvenue
                GreetingText.Text = $"Bonjour {name} !";

                // Ouvre le menu principal
                MenuWindow menu = new MenuWindow();
                menu.Show();

                this.Close();
            }
            else
            {
                GreetingText.Text = "Bonjour !";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string savedName = Properties.Settings.Default.UserName;

            if (!string.IsNullOrWhiteSpace(savedName))
            {
                NameInput.Text = savedName;
                GreetingText.Text = $"Bonjour {savedName} !";
            }
        }

        private void NameInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            GreetingText.Text = "";
        }
    }
}
