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
        public CardRarity Rarity { get; set; }
        public string color { get; set; }
        public string IconPath { get; set; }
        public string Description { get; set; }

        public Card(string name, CardRarity rarity, string iconPath, string description)
        {
            Name = name;
            Rarity = rarity;
            IconPath = iconPath;
            color = GetRarityColor(Rarity);
            Description = description;
        }

        // La liste principale contient maintenant toutes les cartes possibles
        public static List<Card> GetAllPossibleCards()
        {
            return new List<Card>
            {
                // Cartes originales mises à jour
                new Card("Chat Zen", CardRarity.Common, "/Images/Cards/Chat.png","Prends quelques minutes pour respirer profondément et relâcher la pression, comme un chat qui médite."),
                new Card("Chien Focus", CardRarity.Common, "/Images/Cards/Chien.png","Reste fidèle à ta tâche, évite les distractions et récompense-toi après un effort soutenu."),
                new Card("Coq Matinal", CardRarity.Common, "/Images/Cards/Poule.png","Commence ta journée par une tâche importante pour profiter de ton énergie matinale."),
                new Card("Renard Sage", CardRarity.Rare, "/Images/Cards/renard.png","Planifie tes pauses stratégiquement pour garder ton esprit vif et créatif."),

                // Nouvelles cartes ajoutées depuis votre image
                new Card("Cochon Calme", CardRarity.Common, "/Images/Cards/cochon.png","Garde ton espace de travail propre pour éviter l'encombrement mental."),
                new Card("Dauphin Agile", CardRarity.Rare, "/Images/Cards/Dophin.png","Alterner entre différentes tâches peut stimuler ta créativité, mais termine toujours ce que tu commences."),
                new Card("Fourmi Ouvrière", CardRarity.Common, "/Images/Cards/fourmi.png","Divise les gros projets en petites étapes pour avancer régulièrement."),
                new Card("Girafe Sereine", CardRarity.Rare, "/Images/Cards/Giraffe.png","Prends du recul sur tes priorités pour mieux voir l'ensemble de tes objectifs."),
                new Card("Hippopotame Épique", CardRarity.Epic, "/Images/Cards/hipo epic.png","Reste imperturbable face aux distractions, concentre-toi sur une tâche à la fois."),
                new Card("Phoque Paisible", CardRarity.Common, "/Images/Cards/phoc.png","Fais des pauses régulières pour t'étirer et recharger ton énergie."),
                new Card("Rat Rusé", CardRarity.Common, "/Images/Cards/rat.png","Trouve des astuces pour contourner les tentations numériques (mode avion, applis de blocage, etc.)."),
                new Card("Singe Joueur", CardRarity.Common, "/Images/Cards/singe.png","Accorde-toi des moments ludiques entre deux sessions de travail intense pour garder la motivation."),
                new Card("Taupe Travailleuse", CardRarity.Common, "/Images/Cards/taupe.png","Travaille dans un environnement calme et tamisé pour favoriser la concentration."),
                new Card("Têtard Évolutif", CardRarity.Common, "/Images/Cards/Tetard.png","Sois patient avec toi-même, la concentration s'améliore avec la pratique."),
                new Card("Tortue Sage", CardRarity.Rare, "/Images/Cards/Tortue.png","Avance lentement mais sûrement, la régularité est la clé du progrès."),
                new Card("Vache Tranquille", CardRarity.Common, "/Images/Cards/vache.png","Prends le temps de savourer tes réussites, même les plus petites, pour renforcer ta motivation."),

                // Cartes originales sans image correspondante (à remplacer)
                //new Card("Lapin Paisible", CardRarity.Common, "/Images/Cards/lapin_paisible.png"), // Image à ajouter
                //new Card("Panda Méditant", CardRarity.Epic, "/Images/Cards/panda_meditant.png"), // Image à ajouter
                //new Card("Paon Majestueux", CardRarity.Rare, "/Images/Cards/paon_majestueux.png"), // Image à ajouter
                //new Card("Loup Alpha", CardRarity.Epic, "/Images/Cards/loup_alpha.png"), // Image à ajouter
                //new Card("Dragon Ancestral", CardRarity.Legendary, "/Images/Cards/dragon_ancestral.png") // Image à ajouter
            };
        }

        // NOTE : Les nouvelles cartes ne sont pas encore dans les caisses.
        // Vous devrez les ajouter manuellement dans les méthodes GetCaisse ci-dessous.

        public static List<Card> GetCaisse1Cards()
        {
            List < Card >  ListCaisse = new List<Card>
            {
                new Card("Chat Zen", CardRarity.Common, "/Images/Cards/Chat.png",""),
                new Card("Coq Matinal", CardRarity.Common, "/Images/Cards/Poule.png",""),
                new Card("Cochon Calme", CardRarity.Common, "/Images/Cards/cochon.png",""),
                new Card("Cochon Calme", CardRarity.Common, "/Images/Cards/cochon.png",""),
                new Card("Dauphin Agile", CardRarity.Rare, "/Images/Cards/Dophin.png",""),
                new Card("Fourmi Ouvrière", CardRarity.Common, "/Images/Cards/fourmi.png",""),
                new Card("Girafe Sereine", CardRarity.Rare, "/Images/Cards/Giraffe.png",""),
                new Card("Hippopotame Épique", CardRarity.Epic, "/Images/Cards/hipo epic.png",""),
                new Card("Phoque Paisible", CardRarity.Common, "/Images/Cards/phoc.png",""),
            };
            ListCaisse = GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }

        public static List<Card> GetCaisse2Cards()
        {
            List<Card> ListCaisse = new List<Card>
            {
                new Card("Chien Focus", CardRarity.Common, "/Images/Cards/Chien.png",""),
                new Card("Renard Sage", CardRarity.Rare, "/Images/Cards/renard.png",""),
                new Card("Tortue Sage", CardRarity.Rare, "/Images/Cards/Tortue.png",""),
                new Card("Rat Rusé", CardRarity.Common, "/Images/Cards/rat.png",""),
                new Card("Singe Joueur", CardRarity.Common, "/Images/Cards/singe.png",""),
                new Card("Taupe Travailleuse", CardRarity.Common, "/Images/Cards/taupe.png",""),
                new Card("Têtard Évolutif", CardRarity.Common, "/Images/Cards/Tetard.png",""),
                new Card("Vache Tranquille", CardRarity.Common, "/Images/Cards/vache.png",""),
            };
            ListCaisse = GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }

        public static List<Card> GetCaisse3Cards()
        {
            List<Card> ListCaisse = new List<Card>
            {
                new Card("Hippopotame Épique", CardRarity.Epic, "/Images/Cards/hipo epic.png",""),

                //deja present dans la caisse 2
                new Card("Singe Joueur", CardRarity.Common, "/Images/Cards/singe.png",""),
                new Card("Taupe Travailleuse", CardRarity.Common, "/Images/Cards/taupe.png",""),
                new Card("Têtard Évolutif", CardRarity.Common, "/Images/Cards/Tetard.png",""),
                new Card("Tortue Sage", CardRarity.Rare, "/Images/Cards/Tortue.png",""),
                new Card("Vache Tranquille", CardRarity.Common, "/Images/Cards/vache.png",""),
            };
            ListCaisse=GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }

        public static List<Card> filterByOptainedCards(List<Card>  ListCaisse)
        {
            List<Card> cards = Card.GetAllCardsSortedByRarity();

            // Supprimer les cartes déjà présentes dans `cards` (comparaison par Name uniquement)
            var filtered = ListCaisse
                .Where(c => !cards.Any(x => x.Name == c.Name))
                .ToList();

            return filtered;
        }

        public static Card FindCard(string name)
        {
            return GetAllPossibleCards().FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                ?? new Card(name, CardRarity.Common, "/Images/Cards/Chat.png",""); // Fallback sur une image qui existe
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