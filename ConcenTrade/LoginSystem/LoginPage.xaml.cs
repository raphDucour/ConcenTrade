using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Concentrade.LoginSystem;
using System.Threading.Tasks; // Ajoutez cette ligne

namespace Concentrade
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e) // Rendre la méthode asynchrone
        {
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            var user = await UserManager.Login(email, password); // Appeler la méthode asynchrone

            if (user != null)
            {
                await UserManager.LoadProperties(email); // Appeler la méthode asynchrone

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