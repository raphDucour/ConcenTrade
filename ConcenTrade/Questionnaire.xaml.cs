using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    public partial class Questionnaire : Page
    {
        private MainWindow? _mainWindow;

        public Questionnaire()
        {
            InitializeComponent();
        }

        public Questionnaire(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string prenom = NameInput.Text!;
            string age = AgeInput.Text!;
            string sexe = (SexeInput.SelectedItem as ComboBoxItem)?.Content.ToString()!;
            string moment = (MomentInput.SelectedItem as ComboBoxItem)?.Content.ToString()!;
            string distrait = (DistraitInput.SelectedItem as ComboBoxItem)?.Content.ToString()!;

            string resume = $"Nom : {prenom}\n" +
                            $"Âge : {age}\n" +
                            $"Sexe : {sexe}\n" +
                            $"Moment de la journée : {moment}\n" +
                            $"Distraction : {distrait}";

            MessageBox.Show(resume, "Réponses");

            // Navigation ou traitement futur ici
            // this.NavigationService.Navigate(new MenuPage());
        }
    }
}
