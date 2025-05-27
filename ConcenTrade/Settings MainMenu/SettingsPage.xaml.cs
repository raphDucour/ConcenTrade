using Concentrade;
using System.Windows;
using System.Windows.Controls;
using Concentrade.Properties;


namespace ConcenTrade
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            ChargerDonneesUtilisateur();
        }

        private void ChargerDonneesUtilisateur()
        {
            PrenomBox.Text = Settings.Default.UserName;
            AgeBox.Text = Settings.Default.UserAge.ToString();

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
                if(item.Content?.ToString()?.ToLower().Contains(distrait.ToLower()) == true)
                {
                    DistraitCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void Sauvegarder_Click(object sender, RoutedEventArgs e)
        {
            var user = new UserAnswers
            {
                Prenom = PrenomBox.Text,
                Age = AgeBox.Text,
                Moment = (MomentCombo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "",
                Distrait = (DistraitCombo.SelectedItem as ComboBoxItem)?.Content.ToString() ?? ""
            };

            user.SauvegarderDansSettings();
            user.SauvegarderDansLaBaseDeDonnees();

            MessageBox.Show("Informations mises à jour avec succès !");
        }

        private void Retour_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
        }
    }
}
