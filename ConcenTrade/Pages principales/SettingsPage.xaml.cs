using Concentrade;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Concentrade.Properties;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace ConcenTrade
{
    public partial class SettingsPage : Page
    {
        private readonly Regex _dateRegex = new Regex(@"^(\d{0,2})/?\d{0,2}/?\d{0,4}$");
        private readonly AppBlocker _appBlocker;
        private Random _random = new Random();

        public SettingsPage()
        {
            InitializeComponent();
            _appBlocker = ((App)Application.Current).AppBlocker;
            ChargerDonneesUtilisateur();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
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
                    Effect = new BlurEffect()
                };

                double size = _random.Next(5, 40);
                particle.Width = size;
                particle.Height = size;
                particle.Opacity = _random.NextDouble() * 0.4 + 0.1;
                ((BlurEffect)particle.Effect).Radius = _random.Next(5, 15);

                particle.RenderTransform = new TranslateTransform(_random.Next(0, (int)this.ActualWidth), _random.Next(0, (int)this.ActualHeight));

                ParticleCanvas.Children.Add(particle);
                AnimateParticle(particle);
            }
        }

        private void AnimateParticle(Ellipse particle)
        {
            var transform = (TranslateTransform)particle.RenderTransform;

            double endX = _random.NextDouble() > 0.5 ? this.ActualWidth + 100 : -100;
            double endY = _random.Next(0, (int)this.ActualHeight);

            var animX = new DoubleAnimation
            {
                To = endX,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)),
            };

            var animY = new DoubleAnimation
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

            transform.BeginAnimation(TranslateTransform.XProperty, animX);
            transform.BeginAnimation(TranslateTransform.YProperty, animY);
        }

        private void ChargerDonneesUtilisateur()
        {
            PrenomBox.Text = Settings.Default.UserName;
            if (Settings.Default.UserBirthDate != new DateTime(1900, 1, 1))
            {
                DateNaissanceBox.Text = Settings.Default.UserBirthDate.ToString("dd/MM/yyyy");
                MettreAJourAgeAffiche(Settings.Default.UserBirthDate);
            }
            string moment = Settings.Default.BestMoment;
            foreach (ComboBoxItem item in MomentCombo.Items)
            {
                if (item.Content.ToString() == moment)
                {
                    MomentCombo.SelectedItem = item;
                    break;
                }
            }
            string distrait = Settings.Default.Distraction switch
            {
                true => "Oui",
                false => "Non"
            };
            foreach (ComboBoxItem item in DistraitCombo.Items)
            {
                if (item.Content?.ToString()?.ToLower().Contains(distrait.ToLower()) == true)
                {
                    DistraitCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void DateNaissance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text[0]) && e.Text[0] != '/';
        }

        private void DateNaissance_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;

            if (!_dateRegex.IsMatch(text))
            {
                return;
            }

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

            if (DateTime.TryParseExact(text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateNaissance))
            {
                MettreAJourAgeAffiche(dateNaissance);
            }
        }

        private void MettreAJourAgeAffiche(DateTime dateNaissance)
        {
            var today = DateTime.Today;
            var age = today.Year - dateNaissance.Year;
            if (dateNaissance.Date > today.AddYears(-age)) age--;
            AgeActuelBlock.Text = $"{age} ans";
        }

        private bool IsValidDate(string date)
        {
            if (!DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                return false;

            return parsedDate <= DateTime.Today && parsedDate > DateTime.Today.AddYears(-120);
        }

        private void Sauvegarder_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidDate(DateNaissanceBox.Text))
            {
                MessageBox.Show("La date de naissance n'est pas valide. Utilisez le format JJ/MM/AAAA.", "Erreur");
                return;
            }

            var dateNaissance = DateTime.ParseExact(DateNaissanceBox.Text, "dd/MM/yyyy", null);

            var user = new UserAnswers
            {
                Prenom = PrenomBox.Text,
                DateNaissance = DateNaissanceBox.Text,
                Moment = (MomentCombo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "",
                Distrait = (DistraitCombo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? ""
            };

            Settings.Default.UserBirthDate = dateNaissance;

            user.SauvegarderDansSettings();
            user.SauvegarderDansLaBaseDeDonnees();

            MessageBox.Show("Informations mises à jour avec succès !");
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }

        private void GererAppsBloquees_Click(object sender, RoutedEventArgs e)
        {
            string currentUserEmail = Concentrade.Properties.Settings.Default.UserEmail;
            var currentBlockedApps = _appBlocker.GetBlockedApps().ToArray();

            // CHANGEMENT : On charge aussi la liste des applications ignorées
            var currentIgnoredApps = UserManager.LoadIgnoredAppsForUser(currentUserEmail);

            // On passe les deux listes à la fenêtre de configuration
            var settingsWindow = new BlockedAppsSettings(currentBlockedApps, currentIgnoredApps);

            if (settingsWindow.ShowDialog() == true)
            {
                // La sauvegarde des applications bloquées ne change pas
                var newBlockedApps = settingsWindow.BlockedApps;
                _appBlocker.UpdateBlockedApps(newBlockedApps);
                UserManager.SaveBlockedAppsForUser(currentUserEmail, newBlockedApps);

                // CHANGEMENT : On sauvegarde la nouvelle liste des applications ignorées
                var newIgnoredApps = settingsWindow.IgnoredApps;
                UserManager.SaveIgnoredAppsForUser(currentUserEmail, newIgnoredApps);
            }
        }
    }
}