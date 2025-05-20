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
            // MessageBox.Show($"Résolution détectée : {screenWidth} x {screenHeight}");
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

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text;
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Sauvegarde
                Properties.Settings.Default.UserName = name;
                Properties.Settings.Default.Save();

                GreetingText.Text = $"Bonjour {name} !";

                // Ouvre MenuWindow (à condition qu’il existe)
                MenuWindow menu = new MenuWindow();
                menu.Show();
                this.Close();
            }
            else
            {
                GreetingText.Text = "Bonjour !";
            }
        }

        private void NameInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            GreetingText.Text = "";
        }
    }
}
