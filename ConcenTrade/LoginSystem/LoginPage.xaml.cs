using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Concentrade.LoginSystem;

namespace Concentrade
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            var user = UserManager.Login(email, password);

            if (user != null)
            {
                // Sauvegarde l'email de l'utilisateur actuel pour le reste de la session
                UserManager.LoadProperties(email);

                

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
            else
            {
                MessageBox.Show("Identifiants incorrects.");
            }
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SignUpPage());
        }
    }
}