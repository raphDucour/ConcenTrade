using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Threading.Tasks; // Ajoutez cette ligne

namespace Concentrade.LoginSystem
{
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e) // Rendre la méthode asynchrone
        {
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tous les champs sont obligatoires.");
                return;
            }

            bool success = await UserManager.Register(email, password); // Appeler la méthode asynchrone
            if (success)
            {
                MessageBox.Show("Compte créé avec succès, veuillez verifier votre email !");
                this.NavigationService?.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show("il y a eu une erreur");
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new LoginPage());
        }
    }
}