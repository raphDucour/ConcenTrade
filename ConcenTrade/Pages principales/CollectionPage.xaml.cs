using System.Windows;
using System.Windows.Controls;
using Concentrade.Properties;
using Concentrade.Pages_principales.Collection;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales
{
    public partial class CollectionPage : Page
    {
        private string[] cardNames = Card.GetCardNamesArray();

        private int userPoints;

        public CollectionPage()
        {
            InitializeComponent();
            LoadUserPoints();
            InitializeCards();
        }

        private void InitializeCards()
        {
            CardsPanel.Children.Clear();
            
            // Liste de toutes les cartes disponibles
            string[] allCards = new string[]
            {
                "Chat Zen",
                "Chien Focus",
                "Panda Méditant",
                "Renard Sage",
                "Lapin Paisible",
                "Loup Alpha",
                "Coq Matinal",
                "Paon Majestueux",
                "Dragon Ancestral"
            };

            foreach (var cardName in allCards)
            {
                var cardControl = new CardControl();
                cardControl.SetCardName(cardName);
                cardControl.Margin = new Thickness(10);
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
                Card.AddCard(cardName);
                
                // Mettre à jour le tableau avant de rafraîchir
                cardNames = Card.GetCardNamesArray();
                
                CardsPanel.Children.Clear();
                InitializeCards();
                
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