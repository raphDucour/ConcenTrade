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
            MomentInput.SelectedIndex = 0;

            this.Loaded += Page_Loaded;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MomentInput.Focus(); // 👈 donne immédiatement le focus clavier à la ComboBox
        }

        private int _etapeCombo = 0;
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_etapeCombo == 0)
                {
                    // Étape 1 : ouvrir le menu déroulant
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MomentInput.IsDropDownOpen = true;
                    }), System.Windows.Threading.DispatcherPriority.Input);

                    _etapeCombo = 1;
                }
                else if (_etapeCombo == 1)
                {
                    // Étape 2 : fermer le menu et garder le choix
                    MomentInput.IsDropDownOpen = false;
                    _etapeCombo = 2;
                    SuivantButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
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