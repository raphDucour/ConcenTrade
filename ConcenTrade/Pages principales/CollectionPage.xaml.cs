using System.Windows;
using System.Windows.Controls;
using Concentrade.Properties;
using Concentrade.Pages_principales.Collection;
using Concentrade.Collections_de_cartes;
using System.Xml.Linq;

namespace Concentrade.Pages_principales
{
    public partial class CollectionPage : Page
    {
        List<Card> cards = Card.GetAllCards();
       

        private int userPoints;

        public CollectionPage()
        {
            InitializeComponent();
            LoadUserPoints();
            InitializeCards();
        }

        private void InitializeCards()
        {
            foreach (Card card in cards)
            {
                var cardControl = new CardControl();
                cardControl.SetCard(card);
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

        private bool TryBuyCard(Card cardName, int cost)
        {
            if (userPoints >= cost)
            {
                userPoints -= cost;
                SaveUserPoints();
                Card.AddCard(cardName);
                
                // Mettre à jour le tableau avant de rafraîchir
                cards = Card.GetAllCards();
                
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
            TryBuyCard(new Card("Chat Zen", CardRarity.Common, "🐱"), 500);
        }

        private void BuyDog_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard(new Card("Chien Focus", CardRarity.Common, "🐕"), 1000);
        }

        private void BuyPanda_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard(new Card("Panda Méditant", CardRarity.Epic, "🐼"), 750);
        }

        private void BuyFox_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard(new Card("Renard Sage", CardRarity.Rare, "🦊"), 2000);
        }

        private void BuyRabbit_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard(new Card("Lapin Paisible", CardRarity.Common, "🐰"), 600);
        }

        private void BuyWolf_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard(new Card("Loup Alpha", CardRarity.Epic, "🐺"), 3000);
        }

        private void BuyRooster_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard(new Card("Paon Majestueux", CardRarity.Rare, "🦚"), 550);
        }

        private void BuyPeacock_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard(new Card("Coq Matinal", CardRarity.Common, "🐓"), 1500);
        }

        private void BuyDragon_Click(object sender, RoutedEventArgs e)
        {
            TryBuyCard(new Card("Dragon Ancestral", CardRarity.Legendary, "🐲"), 2500);
        }
    }
} 