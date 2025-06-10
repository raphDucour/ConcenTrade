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
            // Initialisez le texte
            WelcomeText.Text = $"Bonjour {userName} !";

            // Démarrer l'animation principale
            await AnimateWelcomeSequence();

            // Attente avant redirection
            await Task.Delay(2000); // Réduit le délai pour une transition plus rapide

            // Lancer l'animation de transition vers MenuPage
            var transitionOutStoryboard = (Storyboard)FindResource("TransitionOutToMenu");
            if (transitionOutStoryboard != null)
            {
                transitionOutStoryboard.Completed += (s, args) =>
                {
                    if (Application.Current.MainWindow is MainWindow main)
                    {
                        main.NavigateTo(new MenuPage());
                    }
                };
                transitionOutStoryboard.Begin();
            }
            else
            {
                // Fallback if storyboard not found
                if (Application.Current.MainWindow is MainWindow main)
                {
                    main.NavigateTo(new MenuPage());
                }
            }
        }

        private async Task AnimateWelcomeSequence()
        {
            // Animations du cercle de méditation
            var meditationCircleScaleUp = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(1.0))
            {
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 }
            };
            var meditationCircleFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.8));

            MeditationCircle.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, meditationCircleScaleUp);
            MeditationCircle.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, meditationCircleScaleUp);
            MeditationCircle.BeginAnimation(OpacityProperty, meditationCircleFadeIn);

            await Task.Delay(300); // Délai avant les vagues

            // Animations des vagues zen
            var wave1FadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.0)) { BeginTime = TimeSpan.FromSeconds(0.2) };
            var wave2FadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.0)) { BeginTime = TimeSpan.FromSeconds(0.4) };

            ZenWave1.BeginAnimation(OpacityProperty, wave1FadeIn);
            ZenWave2.BeginAnimation(OpacityProperty, wave2FadeIn);

            // CHANGEMENT ICI : Appel de la nouvelle méthode AnimateSubtleWave
            AnimateSubtleWave(ZenWave1, "ZenWave1Transform", 3.0, 5, -5); // Durée plus courte, mouvement plus petit
            AnimateSubtleWave(ZenWave2, "ZenWave2Transform", 3.5, -5, 5); // Durée légèrement différente, mouvement inverse

            await Task.Delay(500); // Délai avant le symbole de concentration

            // Animation du symbole de concentration
            var focusSymbolFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.0));
            var focusSymbolScale = new DoubleAnimation(0.5, 1, TimeSpan.FromSeconds(1.0))
            {
                EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 2, Springiness = 2.0 }
            };

            FocusSymbol.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, focusSymbolScale);
            FocusSymbol.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, focusSymbolScale);
            FocusSymbol.BeginAnimation(OpacityProperty, focusSymbolFadeIn);

            await Task.Delay(500); // Délai avant le texte de bienvenue

            // Animation du texte de bienvenue (plus dynamique)
            var textSlideUp = new DoubleAnimation(30, 0, TimeSpan.FromSeconds(1.0))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var textFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.8));

            WelcomeText.RenderTransform.BeginAnimation(TranslateTransform.YProperty, textSlideUp);
            WelcomeText.BeginAnimation(OpacityProperty, textFadeIn);

            await Task.Delay(500); // Délai avant le message de concentration

            // Animation du message de concentration
            var messageSlideUp = new DoubleAnimation(20, 0, TimeSpan.FromSeconds(0.8))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var messageFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.6));

            FocusMessage.RenderTransform.BeginAnimation(TranslateTransform.YProperty, messageSlideUp);
            FocusMessage.BeginAnimation(OpacityProperty, messageFadeIn);

            await Task.Delay(1000); // Attendre la fin des animations avant de permettre la transition
        }

        // Nouvelle méthode pour une animation de vague plus subtile.
        private void AnimateSubtleWave(Path wave, string transformName, double durationSeconds, double fromY, double toY)
        {
            var transform = (TranslateTransform)((TransformGroup)wave.RenderTransform).Children[0]; // Accéder au TranslateTransform

            var waveAnimation = new DoubleAnimation(fromY, toY, TimeSpan.FromSeconds(durationSeconds))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut } // Mouvement plus doux
            };

            transform.BeginAnimation(TranslateTransform.YProperty, waveAnimation);
        }
    }
}