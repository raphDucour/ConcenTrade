using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Concentrade
{
    public partial class QuestionDistrait : Page
    {
        private UserAnswers _answers;

        // Initialise la page de question sur la distraction avec les réponses utilisateur
        public QuestionDistrait(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
            TerminerButton.IsEnabled = false;
            DistraitInput.SelectionChanged += DistraitInput_SelectionChanged;
        }

        // Active le bouton terminer quand une option est sélectionnée
        private void DistraitInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TerminerButton.IsEnabled = DistraitInput.SelectedIndex != -1;
        }

        // Permet de valider avec la touche Entrée
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TerminerButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        // Sauvegarde la réponse et termine le questionnaire
        private void Terminer_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DistraitInput.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Merci de sélectionner une réponse.", "Erreur");
                return;
            }

            _answers.Distrait = selectedItem.Content.ToString()!;
            _answers.SauvegarderDansSettings();
            UserManager.PushIntoBDD_FireAndForget();

            string savedName = Properties.Settings.Default.UserName;
            this.NavigationService?.Navigate(new WelcomePage(savedName));
        }
    }
}