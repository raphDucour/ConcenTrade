using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Concentrade.LoginSystem
{
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tous les champs sont obligatoires.");
                return;
            }

            bool success = UserManager.Register(email, password);
            if (success)
            {
                MessageBox.Show("Compte créé avec succès !");
                this.NavigationService?.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show("Cet email est déjà utilisé.");
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new LoginPage());
        }
    }
}
