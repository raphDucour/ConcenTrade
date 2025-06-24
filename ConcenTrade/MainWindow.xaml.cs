using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Concentrade
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Rendre la fenêtre sans bordure et en plein écran
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.NoResize;

            // Logique de navigation initiale
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.UserEmail))
            {
                MainFrame.Navigate(new LoginPage());
            }
            else
            {
                // On charge les données de l'utilisateur qui est automatiquement connecté.
                string userEmail = Properties.Settings.Default.UserEmail;

                //faire une methode FireAndForget, ou on verifie si tout les champs sont les memes dans la bases de donné et les propriété locale, si c'est les meme rien faire, si c'est pas les memes prendre celui ou il y a plus de cartes en priorité, puis celui ou il y a le plus de points, sinon si c'est les appbloqué qui change on prend celui ou il y a le plus d'app.
                //autre option et de creer une propriété et colone modification date et choisir la plus recente
                UserManager.LoadProperties(userEmail); // Assure que toutes les propriétés sont à jour

                bool questionnaireDone = Properties.Settings.Default.QuestionnaireDone;
                if (questionnaireDone)
                {
                    string savedName = Properties.Settings.Default.UserName;
                    MainFrame.Navigate(new WelcomePage(savedName));
                }
                else
                {
                    MainFrame.Navigate(new QuestionPrenom());
                }
            }
        }

        // Permet de déplacer la fenêtre en cliquant n'importe où
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Logique pour le bouton Fermer
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Logique pour le bouton Agrandir/Restaurer
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        // Logique pour le bouton Réduire
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Méthode de navigation
        public void NavigateTo(Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}