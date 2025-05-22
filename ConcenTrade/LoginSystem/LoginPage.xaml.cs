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
            string input = EmailOrUsernameBox.Text;
            string password = PasswordBox.Password;

            var users = UserManager.LoadUsers();
            var hash = User.HashPassword(password);

            var user = users.Find(u =>
                (u.Email == input || u.Username == input) &&
                u.PasswordHash == hash
            );

            if (user != null)
            {
                MessageBox.Show($"Bienvenue {user.Username} !");
                // ici, tu peux naviguer vers ta page principale
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
