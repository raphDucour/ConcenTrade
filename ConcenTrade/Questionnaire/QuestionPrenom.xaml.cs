using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    public partial class QuestionPrenom : Page
    {
        private UserAnswers _answers;

        // Constructeur vide : utile pour MainWindow ou tests
        public QuestionPrenom()
        {
            InitializeComponent();
            _answers = new UserAnswers(); // par défaut
        }

        // Constructeur avec données
        public QuestionPrenom(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
        }

        private void Suivant_Click(object sender, RoutedEventArgs e)
        {
            _answers.Prenom = NameInput.Text;

            if (string.IsNullOrWhiteSpace(_answers.Prenom))
            {
                MessageBox.Show("Merci de renseigner ton prénom.", "Erreur");
                return;
            }

            // Navigue vers la page suivante (que tu créeras après)
            this.NavigationService?.Navigate(new QuestionAge(_answers));
        }
    }
}
