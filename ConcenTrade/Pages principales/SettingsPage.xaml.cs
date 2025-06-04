using Concentrade;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Concentrade.Properties;

namespace ConcenTrade
{
    public partial class SettingsPage : Page
    {
        private readonly Regex _dateRegex = new Regex(@"^(\d{0,2})/?\d{0,2}/?\d{0,4}$");
        private readonly AppBlocker _appBlocker;

        public SettingsPage()
        {
            InitializeComponent();
            _appBlocker = ((App)Application.Current).AppBlocker;
            ChargerDonneesUtilisateur();
        }

        private void ChargerDonneesUtilisateur()
        {
            PrenomBox.Text = Settings.Default.UserName;

            // Afficher la date de naissance
            if (Settings.Default.UserBirthDate != new DateTime(1900, 1, 1))
            {
                DateNaissanceBox.Text = Settings.Default.UserBirthDate.ToString("dd/MM/yyyy");
                MettreAJourAgeAffiche(Settings.Default.UserBirthDate);
            }

            // Moment préféré
            string moment = Settings.Default.BestMoment;
            foreach (ComboBoxItem item in MomentCombo.Items)
            {
                if (item.Content.ToString() == moment)
                {
                    MomentCombo.SelectedItem = item;
                    break;
                }
            }

            // Distraction
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

            // Vérifier si le texte correspond au format attendu
            if (!_dateRegex.IsMatch(text))
            {
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

            // Mettre à jour l'âge affiché si la date est complète
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

            // Sauvegarder la date de naissance dans les paramètres
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
            var settingsWindow = new BlockedAppsSettings(_appBlocker.GetBlockedApps());
            if (settingsWindow.ShowDialog() == true)
            {
                _appBlocker.UpdateBlockedApps(settingsWindow.BlockedApps);
            }
        }
    }
}
