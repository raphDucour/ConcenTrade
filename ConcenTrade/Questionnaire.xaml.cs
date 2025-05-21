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

            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();


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
            string age = AgeBox.Text;


            int duration = DurationBox.SelectedIndex switch
            {
                0 => 15,
                1 => 25,
                2 => 45,
                3 => 60,
                4 => -1, // personnalisé (à gérer plus tard si besoin)
                _ => 0
            };

            string moment = (MomentBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            string distraction = DistraitOui.IsChecked == true ? "Oui" :
                                 DistraitUnPeu.IsChecked == true ? "Un peu" :
                                 DistraitNon.IsChecked == true ? "Non" : "Non renseigné";

            bool autoStart = AutoStartOui.IsChecked == true;

            // ✅ Enregistrement des infos
            Properties.Settings.Default.UserName = name;
            Properties.Settings.Default.UserAge = age;
            Properties.Settings.Default.FocusDuration = duration;
            Properties.Settings.Default.UserMoment = moment;
            Properties.Settings.Default.UserDistraction = distraction;
            Properties.Settings.Default.UserAutoStart = autoStart;
            Properties.Settings.Default.Save();

            // Navigation
            if (!string.IsNullOrWhiteSpace(name))
                GreetingText.Text = $"Bonjour {name} !";
            else
                GreetingText.Text = "Bonjour !";

            _mainWindow.NavigateTo(new MenuPage());
        }


        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            GreetingText.Text = "";
        }

        private void ResetData_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            GreetingText.Text = "Données réinitialisées.";
        }


    }
}