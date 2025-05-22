using System.Windows;
using System.Windows.Controls;

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