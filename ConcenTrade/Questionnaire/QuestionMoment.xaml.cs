using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    public partial class QuestionMoment : Page
    {
        private UserAnswers _answers;

        public QuestionMoment(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
        }

        private void Suivant_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = MomentInput.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Merci de sélectionner un moment.", "Erreur");
                return;
            }

            _answers.Moment = selectedItem.Content.ToString()!;

            this.NavigationService?.Navigate(new QuestionDistrait(_answers));
        }
    }
}
