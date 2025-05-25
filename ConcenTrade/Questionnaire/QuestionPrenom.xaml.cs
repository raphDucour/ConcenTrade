using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Concentrade
{
    public partial class QuestionPrenom : Page
    {
        private UserAnswers _answers;

        public QuestionPrenom()
        {
            InitializeComponent();
            _answers = new UserAnswers(); // première page, donc on crée les réponses
        }
        private void NameInput_Loaded(object sender, RoutedEventArgs e)
        {
            NameInput.Focus();
            NameInput.SelectAll(); // Facultatif : sélectionne tout le texte si jamais il y a une valeur déjà
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