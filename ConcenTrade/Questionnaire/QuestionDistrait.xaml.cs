using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Concentrade
{
    public partial class QuestionDistrait : Page
    {
        private UserAnswers _answers;

        public QuestionDistrait(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
        }
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TerminerButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
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
            UserManager.PushIntoBDD();

            // ➡️ Tu peux maintenant aller vers un écran de résumé ou Menu
            // this.NavigationService?.Navigate(new QuestionRecap(_answers));


            string savedName = Properties.Settings.Default.UserName;
            this.NavigationService?.Navigate(new WelcomePage(savedName));
            // Ex : navigation finale
            // this.NavigationService?.Navigate(new MainMenu());
        }
    }
}