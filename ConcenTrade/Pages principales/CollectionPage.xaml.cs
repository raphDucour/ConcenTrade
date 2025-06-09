using System.Windows;
using System.Windows.Controls;
using Concentrade.Properties;
using Concentrade.Pages_principales.Collection;

namespace Concentrade.Pages_principales
{
    public partial class CollectionPage : Page
    {
        private readonly string[] cardNames = new[]
        {
            "Chat Zen", "Chien Focus", "Panda Méditant", "Renard Sage",
            "Dragon Concentré", "Hibou Studieux", "Koala Paisible", "Papillon Libre","caca"
        };

        private int userPoints;

        public CollectionPage()
        {
            InitializeComponent();
            LoadUserPoints();
            InitializeCards();
        }

        private void InitializeCards()
        {
            for (int i = 0; i < cardNames.Length; i++)
            {
                var cardControl = new CardControl();
                cardControl.SetCardName(cardNames[i % cardNames.Length]);
                CardsPanel.Children.Add(cardControl);
            }
        }

        private void LoadUserPoints()
        {
            userPoints = 10000; //Settings.Default.Points;
        }

        private void SaveUserPoints()
        {
            Settings.Default.Points = userPoints;
            Settings.Default.Save();
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

        private bool TryBuyCard(string cardName, int cost)
        {
            if (userPoints >= cost)
            {
                userPoints -= cost;
                SaveUserPoints();
                MessageBox.Show($"{cardName} acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void BuyCat_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Chat Zen", 500);
        }

        private void BuyDog_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Chien Focus", 1000);
        }

        private void BuyPanda_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Panda Méditant", 750);
        }

        private void BuyFox_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Renard Sage", 2000);
        }

        private void BuyRabbit_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Lapin Paisible", 600);
        }

        private void BuyWolf_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Loup Alpha", 3000);
        }

        private void BuyRooster_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Coq Matinal", 550);
        }

        private void BuyPeacock_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Paon Majestueux", 1500);
        }

        private void BuyDragon_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard("Dragon Ancestral", 2500);
        }
    }
} 