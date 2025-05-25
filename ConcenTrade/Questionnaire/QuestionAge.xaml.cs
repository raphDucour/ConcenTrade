using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Concentrade
{
    public partial class QuestionAge : Page
    {
        private UserAnswers _answers;

        public QuestionAge(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
        }
        private void AgeInput_Loaded(object sender, RoutedEventArgs e)
        {
            AgeInput.Focus();
            AgeInput.SelectAll(); // Facultatif : sélectionne tout le texte si jamais il y a une valeur déjà
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
            _answers.Age = AgeInput.Text;

            if (string.IsNullOrWhiteSpace(_answers.Age))
            {
                MessageBox.Show("Merci de renseigner ton âge.", "Erreur");
                return;
            }

            // Navigation vers la prochaine page (que tu vas créer après)
            this.NavigationService?.Navigate(new QuestionSexe(_answers));
        }
    }
}