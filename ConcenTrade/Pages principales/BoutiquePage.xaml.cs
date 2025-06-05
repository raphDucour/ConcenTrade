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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Concentrade.Pages_principales
{
    /// <summary>
    /// Logique d'interaction pour BoutiquePage.xaml
    /// </summary>
    public partial class BoutiquePage : Page
    {
        public BoutiquePage()
        {
            InitializeComponent();
        }

        private void RetourButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}
