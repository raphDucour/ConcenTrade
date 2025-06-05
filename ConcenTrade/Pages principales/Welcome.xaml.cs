using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace Concentrade
{
    public partial class WelcomePage : Page
    {
        private readonly string userName;

        public WelcomePage(string name)
        {
            userName = name;
            InitializeComponent();
        }

        private async void Page_Loaded(object? sender, RoutedEventArgs e)
        {
            // Animation des éléments zen
            AnimateZenElements();

            // Animation du symbole de concentration
            await AnimateFocusSymbol();

            // Animation du texte
            WelcomeText.Text = $"Bonjour {userName} !";
            BlurredText.Text = WelcomeText.Text;
            await AnimateWelcomeText();

            // Animation du message de concentration
            await AnimateFocusMessage();

            // Transition de sortie
            var transitionStoryboard = (Storyboard)FindResource("TransitionOut");
            transitionStoryboard.Begin();

            // Attente avant redirection
            await Task.Delay(3000);
            if (Application.Current.MainWindow is MainWindow main)
            {
                main.NavigateTo(new MenuPage());
            }
        }

        private void AnimateZenElements()
        {
            // Animation du cercle de méditation
            var meditationFade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var meditationPulse = new DoubleAnimation(0.95, 1.05, TimeSpan.FromSeconds(4))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            MeditationCircle.BeginAnimation(OpacityProperty, meditationFade);
            MeditationScale.BeginAnimation(ScaleTransform.ScaleXProperty, meditationPulse);
            MeditationScale.BeginAnimation(ScaleTransform.ScaleYProperty, meditationPulse);

            // Animation des vagues zen
            var wave1Fade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var wave2Fade = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2))
            {
                BeginTime = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            ZenWave1.BeginAnimation(OpacityProperty, wave1Fade);
            ZenWave2.BeginAnimation(OpacityProperty, wave2Fade);

            // Animation douce des vagues
            AnimateWave(ZenWave1, 280, 320);
            AnimateWave(ZenWave2, 330, 370);
        }

        private void AnimateWave(Path wave, double minY, double maxY)
        {
            var waveAnimation = new DoubleAnimation(minY, maxY, TimeSpan.FromSeconds(4))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            var geometry = wave.Data as PathGeometry;
            var segment = (geometry?.Figures[0].Segments[0] as BezierSegment);
            
            if (segment != null)
            {
                var story = new Storyboard();
                
                var point1Animation = new PointAnimation(
                    new Point(200, minY),
                    new Point(200, maxY),
                    TimeSpan.FromSeconds(4))
                {
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                
                var point2Animation = new PointAnimation(
                    new Point(400, maxY),
                    new Point(400, minY),
                    TimeSpan.FromSeconds(4))
                {
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(point1Animation, segment);
                Storyboard.SetTargetProperty(point1Animation, new PropertyPath(BezierSegment.Point1Property));
                
                Storyboard.SetTarget(point2Animation, segment);
                Storyboard.SetTargetProperty(point2Animation, new PropertyPath(BezierSegment.Point2Property));

                story.Children.Add(point1Animation);
                story.Children.Add(point2Animation);
                story.Begin();
            }
        }

        private async Task AnimateFocusSymbol()
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var scaleIn = new DoubleAnimation(0.8, 1, TimeSpan.FromSeconds(1.5))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            FocusSymbol.BeginAnimation(OpacityProperty, fadeIn);
            FocusSymbolScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleIn);
            FocusSymbolScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleIn);

            await Task.Delay(750);
        }

        private async Task AnimateWelcomeText()
        {
            // Animation du texte principal
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.2))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var slideUp = new DoubleAnimation(15, 0, TimeSpan.FromSeconds(1.2))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var scaleIn = new DoubleAnimation(0.98, 1, TimeSpan.FromSeconds(1.2))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Animation du texte flou
            var blurFadeIn = new DoubleAnimation(0, 0.3, TimeSpan.FromSeconds(1.2))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var blurSlideUp = new DoubleAnimation(20, 5, TimeSpan.FromSeconds(1.2))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var blurScaleIn = new DoubleAnimation(0.98, 1.02, TimeSpan.FromSeconds(1.2))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            // Démarrage des animations
            WelcomeText.BeginAnimation(OpacityProperty, fadeIn);
            TextTransform.BeginAnimation(TranslateTransform.YProperty, slideUp);
            TextScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleIn);
            TextScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleIn);

            BlurredText.BeginAnimation(OpacityProperty, blurFadeIn);
            BlurredTextTransform.BeginAnimation(TranslateTransform.YProperty, blurSlideUp);
            BlurredTextScale.BeginAnimation(ScaleTransform.ScaleXProperty, blurScaleIn);
            BlurredTextScale.BeginAnimation(ScaleTransform.ScaleYProperty, blurScaleIn);

            await Task.Delay(800);
        }

        private async Task AnimateFocusMessage()
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1))
            {
                BeginTime = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var slideUp = new DoubleAnimation(10, 0, TimeSpan.FromSeconds(1))
            {
                BeginTime = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            FocusMessage.BeginAnimation(OpacityProperty, fadeIn);
            MessageTransform.BeginAnimation(TranslateTransform.YProperty, slideUp);

            await Task.Delay(1000);
        }
    }
}
