using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Concentrade
{
    public partial class QuestionSexe : Page
    {
        private UserAnswers _answers;

        public QuestionSexe(UserAnswers answers)
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
            var selectedItem = SexeInput.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Merci de sélectionner une option.", "Erreur");
                return;
            }

            _answers.Sexe = selectedItem.Content.ToString()!;

            this.NavigationService?.Navigate(new QuestionMoment(_answers));
        }
    }
}