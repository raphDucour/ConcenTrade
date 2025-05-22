using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Concentrade
{
    public partial class QuestionDistrait : Page
    {
        private UserAnswers _answers;

        public QuestionDistrait(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
            DistraitInput.SelectedIndex = 0;

            this.Loaded += Page_Loaded;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DistraitInput.Focus(); // 👈 donne immédiatement le focus clavier à la ComboBox
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
                        DistraitInput.IsDropDownOpen = true;
                    }), System.Windows.Threading.DispatcherPriority.Input);

                    _etapeCombo = 1;
                }
                else if (_etapeCombo == 1)
                {
                    // Étape 2 : fermer le menu et garder le choix
                    DistraitInput.IsDropDownOpen = false;
                    _etapeCombo = 2;
                    TerminerButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
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
            _answers.SauvegarderDansSettings();

            // ➡️ Tu peux maintenant aller vers un écran de résumé ou Menu
            // this.NavigationService?.Navigate(new QuestionRecap(_answers));


            string savedName = Properties.Settings.Default.UserName;
            this.NavigationService?.Navigate(new WelcomePage(savedName));
            // Ex : navigation finale
            // this.NavigationService?.Navigate(new MainMenu());
        }
    }
}