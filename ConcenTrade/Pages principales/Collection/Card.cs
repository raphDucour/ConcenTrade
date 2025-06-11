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

        public static (CardRarity rarity, string icone) GetRarityAndIcone(string name)
        {
            return name switch
            {
                "Chat Zen" => (CardRarity.Common, "🐱"),
                "Lapin Paisible" => (CardRarity.Common, "🐰"),
                "Coq Matinal" => (CardRarity.Common, "🐓"),
                "Chien Focus" => (CardRarity.Common, "🐕"),
                "Panda Méditant" => (CardRarity.Epic, "🐼"),
                "Renard Sage" => (CardRarity.Rare, "🦊"),
                "Paon Majestueux" => (CardRarity.Rare, "🦚"),
                "Loup Alpha" => (CardRarity.Epic, "🐺"),
                "Dragon Ancestral" => (CardRarity.Legendary, "🐲"),
                _ => (CardRarity.Common, "❓")
            };
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

        // Méthodes statiques pour gérer la collection de cartes
        public static List<Card> GetAllCards()
        {
            var cardsString = Settings.Default.Cards;
            if (string.IsNullOrEmpty(cardsString))
                return new List<Card>();

            return cardsString.Split(',')   
                            .Select(name => {
                                var trimmedName = name.Trim();
                                var (rarity, icon) = GetRarityAndIcone(trimmedName);
                                return new Card(name.Trim(), rarity, icon);
                            })
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