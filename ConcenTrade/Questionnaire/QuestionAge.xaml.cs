using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Concentrade
{
    public partial class QuestionAge : Page
    {
        private UserAnswers _answers;
        private readonly Regex _dateRegex = new Regex(@"^(\d{0,2})/?\d{0,2}/?\d{0,4}$");
        private Random _random = new Random();

        public QuestionAge(UserAnswers answers)
        {
            InitializeComponent();
            _answers = answers;
            SuivantButton.IsEnabled = false;
            this.Loaded += QuestionAge_Loaded;
        }

        private void QuestionAge_Loaded(object sender, RoutedEventArgs e)
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

        private void AgeInput_Loaded(object sender, RoutedEventArgs e)
        {
            DateNaissanceInput.Focus();
        }

        private void DateNaissance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text[0]) && e.Text[0] != '/';
        }

        private void DateNaissance_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;

            // Vérifier si le texte correspond au format attendu.
            if (!_dateRegex.IsMatch(text))
            {
                ErrorMessage.Visibility = Visibility.Visible;
                SuivantButton.IsEnabled = false;
                return;
            }

            // Ajouter automatiquement les /
            if (text.Length == 2 && !text.EndsWith("/"))
            {
                textBox.Text = text + "/";
                textBox.CaretIndex = 3;
            }
            else if (text.Length == 5 && !text.EndsWith("/"))
            {
                textBox.Text = text + "/";
                textBox.CaretIndex = 6;
            }

            ErrorMessage.Visibility = Visibility.Collapsed;
            SuivantButton.IsEnabled = IsValidDate(text);
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SuivantButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private bool IsValidDate(string date)
        {
            if (!DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                return false;

            // Vérifier si la date n'est pas dans le futur et si l'âge est raisonnable (moins de 120 ans)
            return parsedDate <= DateTime.Today && parsedDate > DateTime.Today.AddYears(-120);
        }

        private void Suivant_Click(object sender, RoutedEventArgs e)
        {
            var dateNaissance = DateNaissanceInput.Text;

            if (string.IsNullOrWhiteSpace(dateNaissance))
            {
                MessageBox.Show("Merci de renseigner ta date de naissance.", "Erreur");
                return;
            }

            if (!IsValidDate(dateNaissance))
            {
                MessageBox.Show("La date de naissance n'est pas valide. Utilisez le format JJ/MM/AAAA.", "Erreur");
                return;
            }

            _answers.DateNaissance = dateNaissance;
            this.NavigationService?.Navigate(new QuestionMoment(_answers));
        }
    }
}