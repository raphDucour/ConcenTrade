using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SuivantButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
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