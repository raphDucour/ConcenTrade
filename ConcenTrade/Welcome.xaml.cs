using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Concentrade
{
    public partial class WelcomePage : Page
    {
        public WelcomePage(string userName)
        {
            InitializeComponent();
            WelcomeText.Text = $"Bonjour {userName} !";

            Loaded += WelcomePage_Loaded;
        }

        private async void WelcomePage_Loaded(object sender, RoutedEventArgs e)
        {
            // Animation d'apparition fluide (fade + slide)
            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.6));
            TranslateTransform translate = WelcomeText.RenderTransform as TranslateTransform;
            DoubleAnimation slideUp = new DoubleAnimation(20, 0, TimeSpan.FromSeconds(0.6));

            WelcomeText.BeginAnimation(OpacityProperty, fadeIn);
            translate.BeginAnimation(TranslateTransform.YProperty, slideUp);

            // Pause avant navigation
            await Task.Delay(2000);

            // Aller à MenuPage
            if (Application.Current.MainWindow is MainWindow main)
                main.NavigateTo(new MenuPage());
        }
    }
}
