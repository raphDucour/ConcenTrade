using System.Windows;
using System.Windows.Controls;

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

        private void Terminer_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = DistraitInput.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Merci de sélectionner une réponse.", "Erreur");
                return;
            }

            _answers.Distrait = selectedItem.Content.ToString()!;

            // ➡️ Tu peux maintenant aller vers un écran de résumé ou Menu
            // this.NavigationService?.Navigate(new QuestionRecap(_answers));
            MessageBox.Show(
                $"Résumé :\n\nPrénom : {_answers.Prenom}\nÂge : {_answers.Age}\nSexe : {_answers.Sexe}\nMoment : {_answers.Moment}\nDistrait : {_answers.Distrait}",
                "Réponses");

            // Ex : navigation finale
            // this.NavigationService?.Navigate(new MainMenu());
        }
    }
}
