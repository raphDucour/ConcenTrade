using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Concentrade
{
    public partial class QuestionMoment : Page
    {
        private UserAnswers _answers;
        private Random _random = new Random();

        // Initialise la page de question sur le moment de travail avec les réponses utilisateur
        public QuestionMoment(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
            SuivantButton.IsEnabled = false;
            MomentInput.SelectionChanged += MomentInput_SelectionChanged;
            this.Loaded += QuestionMoment_Loaded;
        }

        // Crée et anime les particules lors du chargement de la page
        private void QuestionMoment_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                CreateAndAnimateParticles(10);
            }
        }

        // Crée et anime un nombre spécifique de particules
        private void CreateAndAnimateParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Ellipse particle = new Ellipse
                {
                    Fill = new SolidColorBrush(Colors.White),
                    Effect = new System.Windows.Media.Effects.BlurEffect()
                };

                double size = _random.Next(5, 40);
                particle.Width = size;
                particle.Height = size;
                particle.Opacity = _random.NextDouble() * 0.4 + 0.1;
                ((System.Windows.Media.Effects.BlurEffect)particle.Effect).Radius = _random.Next(5, 15);

                particle.RenderTransform = new System.Windows.Media.TranslateTransform(_random.Next(0, (int)this.ActualWidth), _random.Next(0, (int)this.ActualHeight));

                ParticleCanvas.Children.Add(particle);
                AnimateParticle(particle);
            }
        }

        // Anime une particule individuelle avec un mouvement aléatoire
        private void AnimateParticle(Ellipse particle)
        {
            var transform = (System.Windows.Media.TranslateTransform)particle.RenderTransform;

            double endX = _random.NextDouble() > 0.5 ? this.ActualWidth + 100 : -100;
            double endY = _random.Next(0, (int)this.ActualHeight);

            var animX = new System.Windows.Media.Animation.DoubleAnimation
            {
                To = endX,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)),
            };

            var animY = new System.Windows.Media.Animation.DoubleAnimation
            {
                To = endY,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)),
            };

            animX.Completed += (s, e) =>
            {
                if (this.ActualWidth > 0 && this.ActualHeight > 0)
                {
                    transform.X = _random.NextDouble() > 0.5 ? -50 : this.ActualWidth + 50;
                    transform.Y = _random.Next(0, (int)this.ActualHeight);
                    AnimateParticle(particle);
                }
            };

            transform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animX);
            transform.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, animY);
        }

        // Permet de valider avec la touche Entrée
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SuivantButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        // Sauvegarde la réponse et navigue vers la question suivante
        private void Suivant_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = MomentInput.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Merci de sélectionner un moment.", "Erreur");
                return;
            }

            _answers.Moment = selectedItem.Content.ToString()!;

            this.NavigationService?.Navigate(new QuestionDistrait(_answers));
        }

        // Active le bouton suivant quand une option est sélectionnée
        private void MomentInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SuivantButton.IsEnabled = MomentInput.SelectedIndex != -1;
        }
    }
}