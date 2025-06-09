using System.Windows;
using System.Windows.Controls;
using Concentrade.Properties;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales
{
    public partial class CollectionPage : Page
    {
        private int userPoints;

        public CollectionPage()
        {
            InitializeComponent();
            LoadUserPoints();
            UpdateCardsDisplay();
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

        private void UpdateCardsDisplay()
        {
            var cards = Card.GetAllCards();
            if (cards.Count == 0)
            {
                CardsListText.Text = "Vous n'avez pas encore de cartes dans votre collection.";
            }
            else
            {
                CardsListText.Text = $"Vos cartes ({cards.Count}):\n" + string.Join(", ", cards.Select(c => c.Name));
            }
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
            UpdateCardsDisplay();
        }

        private bool TryBuyCard(string cardName, int cost)
        {
            if (userPoints >= cost)
            {
                userPoints -= cost;
                SaveUserPoints();
                Card.AddCard(cardName);
                MessageBox.Show($"{cardName} acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateCardsDisplay();
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