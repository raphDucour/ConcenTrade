using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    public partial class Questionnaire : Page
    {
        public Questionnaire()
        {
            InitializeComponent();
        }
        private MainWindow? _mainWindow;

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
            string temps = (TempsConcentrationInput.SelectedItem as ComboBoxItem)?.Content.ToString()!;
            string moment = MomentInput.Text;
            string distrait = (DistraitInput.SelectedItem as ComboBoxItem)?.Content.ToString()!;
            string startup = (StartupInput.SelectedItem as ComboBoxItem)?.Content.ToString()!;

            // Tu peux temporairement afficher les infos pour tester
            string resume = $"Nom : {prenom}\nÂge : {age}\nSexe : {sexe}\nTemps : {temps}\nMoment : {moment}\nDistrait : {distrait}\nStartup : {startup}";
            MessageBox.Show(resume, "Réponses");

            // Tu pourras plus tard : 
            // - les sauvegarder dans une base de données
            // - ou dans les Settings
            // - ou les transmettre à une autre page

            // Exemple d'étape suivante :
            // this.NavigationService.Navigate(new MenuPage());
        }
    }
}
