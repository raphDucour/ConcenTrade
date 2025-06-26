using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading.Tasks;

namespace Concentrade.LoginSystem
{
    public partial class SignUpPage : Page
    {
        /// <summary>
        /// Initialise la page d'inscription
        /// </summary>
        public SignUpPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gère le clic sur le bouton d'inscription en validant les données et créant le compte utilisateur
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement</param>
        /// <param name="e">Les arguments de l'événement</param>
        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            SignUpButton.IsEnabled = false;
            try
            {
                string email = EmailBox.Text.Trim();
                string password = PasswordBox.Password.Trim();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Tous les champs sont obligatoires.");
                    return;
                }

                bool success = await UserManager.Register(email, password);
                if (success)
                {
                    MessageBox.Show("Super, veuillez verifier votre email !");
                    this.NavigationService?.Navigate(new LoginPage());
                }
            }
            finally
            {
                SignUpButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Navigue vers la page de connexion
        /// </summary>
        /// <param name="sender">L'objet qui a déclenché l'événement</param>
        /// <param name="e">Les arguments de l'événement</param>
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new LoginPage());
        }
    }
}