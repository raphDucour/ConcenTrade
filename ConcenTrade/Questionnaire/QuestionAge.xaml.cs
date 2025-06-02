using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace Concentrade
{
    public partial class QuestionAge : Page
    {
        private UserAnswers _answers;
        private readonly Regex _dateRegex = new Regex(@"^(\d{0,2})/?\d{0,2}/?\d{0,4}$");

        public QuestionAge(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
        }

        private void AgeInput_Loaded(object sender, RoutedEventArgs e)
        {
            DateNaissanceInput.Focus();
        }

        private void DateNaissance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text[0]) && e.Text[0] != '/';
        }

        private void DateNaissance_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;

            // Vérifier si le texte correspond au format attendu
            if (!_dateRegex.IsMatch(text))
            {
                ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            // Ajouter automatiquement les /
            if (text.Length == 2 && !text.EndsWith("/"))
            {
                textBox.Text = text + "/";
                textBox.CaretIndex = 3;
            }
            else if (text.Length == 5 && !text.EndsWith("/"))
            {
                textBox.Text = text + "/";
                textBox.CaretIndex = 6;
            }

            ErrorMessage.Visibility = Visibility.Collapsed;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SuivantButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private bool IsValidDate(string date)
        {
            if (!DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                return false;

            // Vérifier si la date n'est pas dans le futur et si l'âge est raisonnable (moins de 120 ans)
            return parsedDate <= DateTime.Today && parsedDate > DateTime.Today.AddYears(-120);
        }

        private void Suivant_Click(object sender, RoutedEventArgs e)
        {
            var dateNaissance = DateNaissanceInput.Text;

            if (string.IsNullOrWhiteSpace(dateNaissance))
            {
                MessageBox.Show("Merci de renseigner ta date de naissance.", "Erreur");
                return;
            }

            if (!IsValidDate(dateNaissance))
            {
                MessageBox.Show("La date de naissance n'est pas valide. Utilisez le format JJ/MM/AAAA.", "Erreur");
                return;
            }

            _answers.DateNaissance = dateNaissance;
            this.NavigationService?.Navigate(new QuestionSexe(_answers));
        }
    }
}