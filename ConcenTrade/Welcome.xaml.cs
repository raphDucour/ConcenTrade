using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Concentrade
{
    public partial class WelcomePage : Page
    {
        private string userName;

        public WelcomePage(string name)
        {
            InitializeComponent();
            userName = name;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Texte animé
            WelcomeText.Text = $"Bonjour {userName} !";
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.8)) { EasingFunction = new QuadraticEase() };
            var slideUp = new DoubleAnimation(40, 0, TimeSpan.FromSeconds(0.8)) { EasingFunction = new QuadraticEase() };

            WelcomeText.BeginAnimation(OpacityProperty, fadeIn);
            
            ((TranslateTransform)WelcomeText.RenderTransform).BeginAnimation(TranslateTransform.YProperty, slideUp);

            // Halo pulsant
            var pulse = new DoubleAnimationUsingKeyFrames();
            pulse.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            pulse.KeyFrames.Add(new EasingDoubleKeyFrame(1.1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));
            pulse.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));
            pulse.RepeatBehavior = RepeatBehavior.Forever;

            PulseEffect.BeginAnimation(ScaleTransform.ScaleXProperty, pulse);
            PulseEffect.BeginAnimation(ScaleTransform.ScaleYProperty, pulse);

            // Dégradé animé
            var colorShift1 = new ColorAnimation(Color.FromRgb(108, 99, 255), Color.FromRgb(163, 137, 244), TimeSpan.FromSeconds(4))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            var colorShift2 = new ColorAnimation(Color.FromRgb(163, 137, 244), Color.FromRgb(60, 59, 91), TimeSpan.FromSeconds(4))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Stop1.BeginAnimation(GradientStop.ColorProperty, colorShift1);
            Stop2.BeginAnimation(GradientStop.ColorProperty, colorShift2);

            // Attente avant redirection
            await Task.Delay(2500);
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.NavigateTo(new MenuPage());
            }
        }
    }
}
