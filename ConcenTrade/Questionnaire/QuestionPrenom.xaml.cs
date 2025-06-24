using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Concentrade
{
    public partial class QuestionPrenom : Page
    {
        private UserAnswers _answers;
        private Random _random = new Random();

        public QuestionPrenom()
        {
            InitializeComponent();
            _answers = new UserAnswers(); // première page, donc on crée les réponses
            this.Loaded += QuestionPrenom_Loaded;
            NameInput.TextChanged += NameInput_TextChanged;
            SuivantButton.IsEnabled = false;
        }

        private void NameInput_Loaded(object sender, RoutedEventArgs e)
        {
            NameInput.Focus();
            NameInput.SelectAll(); // Facultatif : sélectionne tout le texte si jamais il y a une valeur déjà
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SuivantButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void QuestionPrenom_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                CreateAndAnimateParticles(10);
            }
        }

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

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            SuivantButton.IsEnabled = !string.IsNullOrWhiteSpace(NameInput.Text);
        }

        private void Suivant_Click(object sender, RoutedEventArgs e)
        {
            _answers.Prenom = NameInput.Text;

            if (string.IsNullOrWhiteSpace(_answers.Prenom))
            {
                MessageBox.Show("Merci de renseigner ton prénom.", "Erreur");
                return;
            }

            // Navigue vers la page suivante (que tu créeras après)
            this.NavigationService?.Navigate(new QuestionAge(_answers));
        }
    }
}