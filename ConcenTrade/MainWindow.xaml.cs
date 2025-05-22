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
            
            MainFrame.Navigate(new LoginPage());
            
        }
        public void NavigateTo(Page page)
        {
            MainFrame.Navigate(page);
        }

    }
}
