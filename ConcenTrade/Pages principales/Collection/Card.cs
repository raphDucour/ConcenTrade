using System;
using System.Collections.Generic;
using System.Linq;
using Concentrade.Properties;

namespace Concentrade.Collections_de_cartes
{
    public enum CardRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    public class Card
    {
        public string Name { get; set; }
        public bool IsFavorite { get; set; }
        public CardRarity Rarity { get; set; }
        public string color { get; set; }
        public string icone { get; set; }

        public Card(string name,CardRarity rarity, string icon)
        {
            Name = name;
            IsFavorite = false;
            Rarity = rarity;
            icone = icon;
            color = GetRarityColor(Rarity);
        }

        // Méthodes statiques pour gérer la collection de cartes
        public static List<Card> GetAllPossibleCards()
        {
            return new List<Card>
            {
                new Card("Chat Zen",CardRarity.Common, "🐱"),
                new Card("Lapin Paisible",CardRarity.Common, "🐰"),
                new Card("Coq Matinal",CardRarity.Common, "🐓"),
                new Card("Chien Focus",CardRarity.Common, "🐕"),
                new Card("Panda Méditant",CardRarity.Epic, "🐼"),
                new Card("Renard Sage",CardRarity.Rare, "🦊"),
                new Card("Paon Majestueux",CardRarity.Rare, "🦚"),
                new Card("Loup Alpha",CardRarity.Epic, "🐺"),
                new Card("Dragon Ancestral",CardRarity.Legendary, "🐲")
            };
        }
        public static List<Card> GetCaisse1Cards()
        {
            return new List<Card>
            {
                new Card("Chat Zen",CardRarity.Common, "🐱"),
                new Card("Lapin Paisible",CardRarity.Common, "🐰"),
                new Card("Coq Matinal",CardRarity.Common, "🐓")
            };
        }
        public static List<Card> GetCaisse2Cards()
        {
            return new List<Card>
            {
                new Card("Chien Focus",CardRarity.Common, "🐕"),
                new Card("Panda Méditant",CardRarity.Epic, "🐼"),
                new Card("Renard Sage",CardRarity.Rare, "🦊"),
            };
        }

        public static List<Card> GetCaisse3Cards()
        {
            return new List<Card>
            {
                new Card("Paon Majestueux",CardRarity.Rare, "🦚"),
                new Card("Loup Alpha",CardRarity.Epic, "🐺"),
                new Card("Dragon Ancestral",CardRarity.Legendary, "🐲")
            };
        }



        public static Card FindCard(string name)
        {
            return GetAllPossibleCards().FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) 
                ?? new Card(name, CardRarity.Common, "❓"); // Carte par défaut si non trouvée
        }

        public static string GetRarityColor(CardRarity rarity)
        {
            return rarity switch
            {

                CardRarity.Common => "#7FB3F5",    // Bleu clair
                CardRarity.Rare => "#CD853F",      // Orange mat/brun (Peru)
                CardRarity.Epic => "#9B4DCA",      // Violet
                CardRarity.Legendary => "#FFD700",  // Jaune doré/shiny
                _ => "#7FB3F5"                     // Bleu clair par défaut
            };
        }

        private static int GetRarityOrder(CardRarity rarity)
        {
            return rarity switch
            {
                CardRarity.Common => 0,
                CardRarity.Rare => 1,
                CardRarity.Epic => 2,
                CardRarity.Legendary => 3,
                _ => -1
            };
        }

        public static List<Card> GetSortedByRarity(List<Card> cards)
        {
            return cards.OrderBy(card => GetRarityOrder(card.Rarity))
                       .ThenBy(card => card.Name)  // Tri secondaire par nom pour les cartes de même rareté
                       .ToList();
        }

        public static List<Card> GetAllCardsSortedByRarity()
        {
            return GetSortedByRarity(GetAllCards());
        }

        public static List<Card> GetAllPossibleCardsSortedByRarity()
        {
            return GetSortedByRarity(GetAllPossibleCards());
        }

        //carte du deck
        public static List<Card> GetAllCards()
        {
            var cardsString = Settings.Default.Cards;
            if (string.IsNullOrEmpty(cardsString))
                return new List<Card>();

            return cardsString.Split(',')
                            .Select(name => FindCard(name.Trim()))
                            .ToList();
        }

        public static string[] GetCardNamesArray()
        {
            string cardsString = Settings.Default.Cards;
            if (string.IsNullOrEmpty(cardsString))
                return new string[0];

            return cardsString.Split(',');
        }

        public static void AddCard(Card name)
        {
            var cards = GetAllCards();
            cards.Add(name);
            SaveCards(cards);
        }

        public static void RemoveCard(string name)
        {
            var cards = GetAllCards();
            cards.RemoveAll(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            SaveCards(cards);
        }

        public static void ToggleFavorite(string name)
        {
            var cards = GetAllCards();
            var card = cards.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (card != null)
            {
                card.IsFavorite = !card.IsFavorite;
                SaveCards(cards);
            }
        }

        public static List<Card> GetFavoriteCards()
        {
            return GetAllCards().Where(c => c.IsFavorite).ToList();
        }

        

        public static List<Card> SearchCards(string searchTerm)
        {
            return GetAllCards()
                .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }


        private static void SaveCards(List<Card> cards)
        {
            var cardsString = string.Join(",", cards.Select(c => c.Name));
            Settings.Default.Cards = cardsString;
            Settings.Default.Save();
        }
    }
} 