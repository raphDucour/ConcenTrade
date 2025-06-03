using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Concentrade
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // ✅ Affiche les boutons système (plein écran propre)
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.CanResize;
            this.Background = new SolidColorBrush(Color.FromRgb(223, 255, 232)); // Vert menthe clair

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.UserEmail)) MainFrame.Navigate(new LoginPage());
            else
            {
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
        public void NavigateTo(Page page)
        {
            MainFrame.Navigate(page);
        }

    }
}
