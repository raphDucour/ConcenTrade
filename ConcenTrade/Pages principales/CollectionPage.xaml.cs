using System.Windows;
using System.Windows.Controls;

namespace Concentrade.Pages_principales
{
    public partial class CollectionPage : Page
    {
        private int userPoints = 1000;

        public CollectionPage()
        {
            InitializeComponent();
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.GoBack();
        }

        private void AcheterButton_Click(object sender, RoutedEventArgs e)
        {
            AchatOverlay.Visibility = Visibility.Visible;
        }

        private void CloseAchatOverlay_Click(object sender, RoutedEventArgs e)
        {
            AchatOverlay.Visibility = Visibility.Collapsed;
        }

        private void BuyCat_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 500)
            {
                userPoints -= 500;
                MessageBox.Show("Chat Zen acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuyDog_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 1000)
            {
                userPoints -= 1000;
                MessageBox.Show("Chien Focus acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuyPanda_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 750)
            {
                userPoints -= 750;
                MessageBox.Show("Panda Méditant acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuyFox_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 2000)
            {
                userPoints -= 2000;
                MessageBox.Show("Renard Sage acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuyRabbit_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 600)
            {
                userPoints -= 600;
                MessageBox.Show("Lapin Paisible acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuyWolf_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 3000)
            {
                userPoints -= 3000;
                MessageBox.Show("Loup Alpha acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuyRooster_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 550)
            {
                userPoints -= 550;
                MessageBox.Show("Coq Matinal acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuyPeacock_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 1500)
            {
                userPoints -= 1500;
                MessageBox.Show("Paon Majestueux acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuyDragon_Click(object sender, RoutedEventArgs e)
        {
            if (userPoints >= 2500)
            {
                userPoints -= 2500;
                MessageBox.Show("Dragon Ancestral acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 