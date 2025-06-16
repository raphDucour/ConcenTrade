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
        public string IconPath { get; set; }

        public Card(string name, CardRarity rarity, string iconPath)
        {
            Name = name;
            IsFavorite = false;
            Rarity = rarity;
            IconPath = iconPath;
            color = GetRarityColor(Rarity);
        }

        // Méthodes statiques pour gérer la collection de cartes
        public static List<Card> GetAllPossibleCards()
        {
            return new List<Card>
            {
                new Card("Chat Zen", CardRarity.Common, "/Images/Cards/cat_zen.png"),
                new Card("Lapin Paisible", CardRarity.Common, "/Images/Cards/cat_zen.png"),
                new Card("Coq Matinal", CardRarity.Common, "/Images/Cards/cat_zen.png"),
                new Card("Chien Focus", CardRarity.Common, "/Images/Cards/cat_zen.png"),
                new Card("Panda Méditant", CardRarity.Epic, "/Images/Cards/cat_zen.png"),
                new Card("Renard Sage", CardRarity.Rare, "/Images/Cards/cat_zen.png"),
                new Card("Paon Majestueux", CardRarity.Rare, "/Images/Cards/cat_zen.png"),
                new Card("Loup Alpha", CardRarity.Epic, "/Images/Cards/cat_zen.png"),
                new Card("Dragon Ancestral", CardRarity.Legendary, "/Images/Cards/cat_zen.png")
            };
        }

        public static List<Card> GetCaisse1Cards()
        {
            return new List<Card>
            {
                new Card("Chat Zen", CardRarity.Common, "/Images/Cards/cat_zen.png"),
                new Card("Lapin Paisible", CardRarity.Common, "/Images/Cards/cat_zen.png"),
                new Card("Coq Matinal", CardRarity.Common, "/Images/Cards/cat_zen.png")
            };
        }

        public static List<Card> GetCaisse2Cards()
        {
            return new List<Card>
            {
                new Card("Chien Focus", CardRarity.Common, "/Images/Cards/cat_zen.png"),
                new Card("Panda Méditant", CardRarity.Epic, "/Images/Cards/cat_zen.png"),
                new Card("Renard Sage", CardRarity.Rare, "/Images/Cards/cat_zen.png"),
            };
        }

        public static List<Card> GetCaisse3Cards()
        {
            return new List<Card>
            {
                new Card("Paon Majestueux", CardRarity.Rare, "/Images/Cards/cat_zen.png"),
                new Card("Loup Alpha", CardRarity.Epic, "/Images/Cards/cat_zen.png"),
                new Card("Dragon Ancestral", CardRarity.Legendary, "/Images/Cards/cat_zen.png")
            };
        }

        public static Card FindCard(string name)
        {
            return GetAllPossibleCards().FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                ?? new Card(name, CardRarity.Common, "/Images/Cards/cat_zen.png");
        }

        public static string GetRarityColor(CardRarity rarity)
        {
            return rarity switch
            {
                CardRarity.Common => "#7FB3F5",
                CardRarity.Rare => "#CD853F",
                CardRarity.Epic => "#9B4DCA",
                CardRarity.Legendary => "#FFD700",
                _ => "#7FB3F5"
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
                       .ThenBy(card => card.Name)
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

        public static List<Card> GetAllCards()
        {
            var cardsString = Settings.Default.Cards;
            if (string.IsNullOrEmpty(cardsString))
                return new List<Card>();

            return cardsString.Split(',')
                             .Select(name => FindCard(name.Trim()))
                             .ToList();
        }

        public static void AddCard(Card name)
        {
            var cards = GetAllCards();
            cards.Add(name);
            SaveCards(cards);
        }

        private static void SaveCards(List<Card> cards)
        {
            var cardsString = string.Join(",", cards.Select(c => c.Name));
            Settings.Default.Cards = cardsString;
            Settings.Default.Save();
        }
    }
}