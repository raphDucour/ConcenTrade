using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    public partial class Questionnaire : Page
    {
        private MainWindow _mainWindow;

        public Questionnaire(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
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
                Properties.Settings.Default.UserName = name;
                Properties.Settings.Default.Save();

                GreetingText.Text = $"Bonjour {name} !";

                // Navigation vers MenuWindow si tu l’as converti en Page
                _mainWindow.NavigateTo(new MenuPage()); // Ou utilise MenuWindow.ShowDialog() si c’est une Window
            }
            else
            {
                GreetingText.Text = "Bonjour !";
            }
        }

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            GreetingText.Text = "";
        }
    }
}