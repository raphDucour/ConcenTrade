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

            string savedName = Properties.Settings.Default.UserName;

            if (!string.IsNullOrWhiteSpace(savedName))
            {
                // Affiche la page de bienvenue avant d’aller au menu
                MainFrame.Navigate(new WelcomePage(savedName));
            }
            else
            {

                // Charger la première page (Questionnaire)
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
