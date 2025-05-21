using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Concentrade
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // METTRE L'APPLICATION EN PLEIN ÉCRAN
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.NoResize;
            this.Background = new SolidColorBrush(Color.FromRgb(223, 255, 232)); // Vert menthe apaisant

            string savedName = Properties.Settings.Default.UserName;

            if (!string.IsNullOrWhiteSpace(savedName))
            {
                MainFrame.Navigate(new WelcomePage(savedName));
            }
            else
            {
                MainFrame.Navigate(new Questionnaire(this));
            }
        }


        // Méthode pour changer de page (ex. depuis Questionnaire)
        public void NavigateTo(Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}
