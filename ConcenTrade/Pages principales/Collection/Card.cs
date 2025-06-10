using System;
using System.Collections.Generic;
using System.Linq;
using Concentrade.Properties;

namespace Concentrade.Collections_de_cartes
{
    public enum CardRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public class Card
    {
        public string Name { get; set; }
        public bool IsFavorite { get; set; }

        public Card(string name)
        {
            Name = name;
            IsFavorite = false;
        }

        public CardRarity Rarity => GetRarity(Name);

        public static CardRarity GetRarity(string name)
        {
            return name switch
            {
                "Chat Zen" => CardRarity.Common,
                "Lapin Paisible" => CardRarity.Common,
                "Coq Matinal" => CardRarity.Common,
                "Chien Focus" => CardRarity.Uncommon,
                "Panda Méditant" => CardRarity.Uncommon,
                "Renard Sage" => CardRarity.Rare,
                "Paon Majestueux" => CardRarity.Rare,
                "Loup Alpha" => CardRarity.Epic,
                "Dragon Ancestral" => CardRarity.Legendary,
                _ => CardRarity.Common // Par défaut, une carte est commune
            };
        }

        public static string GetRarityColor(CardRarity rarity)
        {
            return rarity switch
            {
                CardRarity.Common => "#2A2A2A",
                CardRarity.Uncommon => "#2B4C8C",
                CardRarity.Rare => "#6B2B8C",
                CardRarity.Epic => "#8C2B6B",
                CardRarity.Legendary => "#8C6B2B",
                _ => "#2A2A2A"
            };
        }

        // Méthodes statiques pour gérer la collection de cartes
        public static List<Card> GetAllCards()
        {
            var cardsString = Settings.Default.Cards;
            if (string.IsNullOrEmpty(cardsString))
                return new List<Card>();

            return cardsString.Split(',')
                            .Select(name => new Card(name.Trim()))
                            .ToList();
        }

        public static string[] GetCardNamesArray()
        {
            string cardsString = Settings.Default.Cards;
            if (string.IsNullOrEmpty(cardsString))
                return new string[0];

            return cardsString.Split(',');
        }

        public static void AddCard(string name)
        {
            var cards = GetAllCards();
            
            

            cards.Add(new Card(name));
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