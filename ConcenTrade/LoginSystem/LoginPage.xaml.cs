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
            string input = EmailBox.Text;
            string password = PasswordBox.Password;

            var users = UserManager.LoadUsers();
            var hash = User.HashPassword(password);

            var user = users.Find(u =>
                u.Email == input  &&
                u.PasswordHash == hash
            );

            if (user != null)
            {
                bool QuestionnaireDone = Properties.Settings.Default.QuestionnaireDone;
                if (QuestionnaireDone)
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
            // Navigation vers la page d’inscription qu’on fera ensuite
            this.NavigationService.Navigate(new SignUpPage()); // à créer plus tard
        }
    }
}
