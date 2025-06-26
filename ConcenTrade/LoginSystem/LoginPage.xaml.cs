using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Concentrade.LoginSystem;
using System.Threading.Tasks;

namespace Concentrade
{
    public partial class LoginPage : Page
    {
        /// <summary>
        /// Initialise la page de connexion
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gère le clic sur le bouton de connexion en validant les identifiants et naviguant vers la page appropriée
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement</param>
        /// <param name="e">Les arguments de l'événement</param>
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            bool success = await UserManager.Login(email, password);

            if (success)
            {
                bool questionnaireDone = Properties.Settings.Default.QuestionnaireDone;
                if (questionnaireDone)
                {
                    string savedName = Properties.Settings.Default.UserName;
                    this.NavigationService?.Navigate(new WelcomePage(savedName));
                }
                else
                {
                    this.NavigationService?.Navigate(new QuestionPrenom());
                }
            }
            // Si la connexion échoue, le message d'erreur est déjà affiché dans UserManager.Login
        }

        /// <summary>
        /// Navigue vers la page d'inscription
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement</param>
        /// <param name="e">Les arguments de l'événement</param>
        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SignUpPage());
        }
    }
}