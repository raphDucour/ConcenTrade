using System.Windows;
using System.Windows.Controls;
using Concentrade.Properties;
using Concentrade.Pages_principales.Collection;
using Concentrade.Collections_de_cartes;
using System.Xml.Linq;
using System.Linq;

namespace Concentrade.Pages_principales
{
    public partial class CollectionPage : Page
    {
        List<Card> cards = Card.GetAllCardsSortedByRarity();
       

        private int userPoints;

        public CollectionPage()
        {
            InitializeComponent();
            LoadUserPoints();
            InitializeCards();
        }

        private void InitializeCards()
        {
            // Grouper les cartes par nom et compter leur occurrence
            var cardGroups = cards.GroupBy(c => c.Name)
                                .ToDictionary(g => g.Key, g => g.Count());

            foreach (Card card in cards.DistinctBy(c => c.Name)) // Ne prend qu'une carte de chaque type
            {
                var cardControl = new CardControl();
                cardControl.SetCard(card);
                
                // Ajouter l'effet d'empilement si on a plus d'une carte
                if (cardGroups[card.Name] > 1)
                {
                    cardControl.AddStackedCards(cardGroups[card.Name] - 1);
                }
                
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
            this.NavigationService.Navigate(new MenuPage());
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
                cards = Card.GetAllCardsSortedByRarity();
                
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
            this.NavigationService?.Navigate(new Caisse(1)); // Caisse Poules
        }

        private void BuyDog_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new Caisse(2)); // Caisse QoC
        }

        private void BuyDragon_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new Caisse(3)); // Caisse Dragon
        }
    }
} 