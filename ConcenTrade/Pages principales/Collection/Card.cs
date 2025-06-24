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
                // Communes
                new Card("Chat Zen", CardRarity.Common, "/Images/Cards/communes/Chat.png","Prends quelques minutes pour respirer profondément et relâcher la pression, comme un chat qui médite."),
                new Card("Chien Focus", CardRarity.Common, "/Images/Cards/communes/Chien.png","Reste fidèle à ta tâche, évite les distractions et récompense-toi après un effort soutenu."),
                new Card("Coq Matinal", CardRarity.Common, "/Images/Cards/communes/Poule.png","Commence ta journée par une tâche importante pour profiter de ton énergie matinale."),
                new Card("Renard Sage", CardRarity.Common, "/Images/Cards/communes/renard.png","Planifie tes pauses stratégiquement pour garder ton esprit vif et créatif."),
                new Card("Cochon Calme", CardRarity.Common, "/Images/Cards/communes/cochon.png","Garde ton espace de travail propre pour éviter l'encombrement mental."),
                new Card("Dauphin Agile", CardRarity.Common, "/Images/Cards/communes/Dophin.png","Alterner entre différentes tâches peut stimuler ta créativité, mais termine toujours ce que tu commences."),
                new Card("Fourmi Ouvrière", CardRarity.Common, "/Images/Cards/communes/fourmi.png","Divise les gros projets en petites étapes pour avancer régulièrement."),
                new Card("Girafe Sereine", CardRarity.Common, "/Images/Cards/communes/Giraffe.png","Prends du recul sur tes priorités pour mieux voir l'ensemble de tes objectifs."),
                new Card("Phoque Paisible", CardRarity.Common, "/Images/Cards/communes/phoc.png","Fais des pauses régulières pour t'étirer et recharger ton énergie."),
                new Card("Rat Rusé", CardRarity.Common, "/Images/Cards/communes/rat.png","Trouve des astuces pour contourner les tentations numériques (mode avion, applis de blocage, etc.)."),
                new Card("Singe Joueur", CardRarity.Common, "/Images/Cards/communes/singe.png","Accorde-toi des moments ludiques entre deux sessions de travail intense pour garder la motivation."),
                new Card("Taupe Travailleuse", CardRarity.Common, "/Images/Cards/communes/taupe.png","Travaille dans un environnement calme et tamisé pour favoriser la concentration."),
                new Card("Têtard Évolutif", CardRarity.Common, "/Images/Cards/communes/Tetard.png","Sois patient avec toi-même, la concentration s'améliore avec la pratique."),
                new Card("Tortue Sage", CardRarity.Rare, "/Images/Cards/communes/Tortue.png","Avance lentement mais sûrement, la régularité est la clé du progrès."),
                new Card("Vache Tranquille", CardRarity.Common, "/Images/Cards/communes/vache.png","Prends le temps de savourer tes réussites, même les plus petites, pour renforcer ta motivation."),

                // rares
                new Card("Âne Concentré", CardRarity.Epic, "/Images/Cards/rares/anne rare.png","Ne laisse pas le hasard décider pour toi. Planifie ta session avant de commencer."),
                new Card("Cafard Chillax", CardRarity.Epic, "/Images/Cards/rares/cafard rare.png","Accorde-toi de vraies pauses pour mieux repartir."),
                new Card("Caméléon Focus", CardRarity.Epic, "/Images/Cards/rares/cameleon rare.png","Adapte ton environnement à ta tâche."),
                new Card("Chef Ornitho", CardRarity.Epic, "/Images/Cards/rares/cuisinier rare.png","La concentration, c’est comme une cuisson : à feu doux mais constant."),
                new Card("Professeur Noisette", CardRarity.Epic, "/Images/Cards/rares/ecureil rare.png","Crée-toi un coin calme, rien qu’à toi."),
                new Card("Flamàn l’Organisé", CardRarity.Epic, "/Images/Cards/rares/flaman rose rare.png","Adopte une posture royale."),
                new Card("Koala Cool", CardRarity.Epic, "/Images/Cards/rares/koala rare.png","Adopte la lenteur consciente : prends de petites pauses régulières pour respirer profondément et revenir plus frais à ta tâche."),
                new Card("Œil de Mouche", CardRarity.Epic, "/Images/Cards/rares/mouche rare.png","Comme une mouche aux yeux perçants, garde ton attention fixée sur un seul objectif à la fois pour maximiser ton efficacité."),
                new Card("Mélodie du Panda", CardRarity.Epic, "/Images/Cards/rares/panda rare.png","Laisse-toi porter par le rythme de ta respiration, comme les notes d’une flûte, pour apaiser ton esprit et retrouver ta concentration."),
                new Card("Verre de Livre", CardRarity.Epic, "/Images/Cards/rares/verre de terre rare.png","Plonge dans ta lecture comme ce ver dans son livre — lentement, mais avec appétit ! Et surtout, évite de te faire dévorer par la procrastination."),
                new Card("Rayures en Pause", CardRarity.Epic, "/Images/Cards/rares/zebre rare.png","Chille comme ce zèbre : parfois, s’asseoir et déconnecter, c’est le meilleur moyen de recharger ses batteries."),

                // epics
                new Card("Hippo Boss", CardRarity.Epic, "/Images/Cards/epics/hipo epic.png","?"),
                new Card("Aigle", CardRarity.Epic, "/Images/Cards/epics/aigle EPIC.png","?"),
                new Card("Hibou", CardRarity.Epic, "/Images/Cards/epics/hibou EPIC.png","?"),
                new Card("Elephant", CardRarity.Epic, "/Images/Cards/epics/elephant epic.png","?"),
                new Card("Lezard", CardRarity.Epic, "/Images/Cards/epics/lezard epic.png","?"),
                new Card("Pelican", CardRarity.Epic, "/Images/Cards/epics/pelican EPIC.png","?"),

                // legendaires


                // legendaires

            };
        }

        // NOTE : Les nouvelles cartes ne sont pas encore dans les caisses.
        // Vous devrez les ajouter manuellement dans les méthodes GetCaisse ci-dessous.

        public static List<Card> GetCaisse1Cards()
        {
            List < Card >  ListCaisse = new List<Card>
            {
                // Communes
                new Card("Chat Zen", CardRarity.Common, "/Images/Cards/communes/Chat.png","Prends quelques minutes pour respirer profondément et relâcher la pression, comme un chat qui médite."),
                new Card("Chien Focus", CardRarity.Common, "/Images/Cards/communes/Chien.png","Reste fidèle à ta tâche, évite les distractions et récompense-toi après un effort soutenu."),
                new Card("Coq Matinal", CardRarity.Common, "/Images/Cards/communes/Poule.png","Commence ta journée par une tâche importante pour profiter de ton énergie matinale."),
                new Card("Renard Sage", CardRarity.Common, "/Images/Cards/communes/renard.png","Planifie tes pauses stratégiquement pour garder ton esprit vif et créatif."),
                new Card("Cochon Calme", CardRarity.Common, "/Images/Cards/communes/cochon.png","Garde ton espace de travail propre pour éviter l'encombrement mental."),
                new Card("Dauphin Agile", CardRarity.Common, "/Images/Cards/communes/Dophin.png","Alterner entre différentes tâches peut stimuler ta créativité, mais termine toujours ce que tu commences."),
                new Card("Fourmi Ouvrière", CardRarity.Common, "/Images/Cards/communes/fourmi.png","Divise les gros projets en petites étapes pour avancer régulièrement."),
                new Card("Girafe Sereine", CardRarity.Common, "/Images/Cards/communes/Giraffe.png","Prends du recul sur tes priorités pour mieux voir l'ensemble de tes objectifs."),
                new Card("Phoque Paisible", CardRarity.Common, "/Images/Cards/communes/phoc.png","Fais des pauses régulières pour t'étirer et recharger ton énergie."),
                new Card("Rat Rusé", CardRarity.Common, "/Images/Cards/communes/rat.png","Trouve des astuces pour contourner les tentations numériques (mode avion, applis de blocage, etc.)."),
                new Card("Singe Joueur", CardRarity.Common, "/Images/Cards/communes/singe.png","Accorde-toi des moments ludiques entre deux sessions de travail intense pour garder la motivation."),
                new Card("Taupe Travailleuse", CardRarity.Common, "/Images/Cards/communes/taupe.png","Travaille dans un environnement calme et tamisé pour favoriser la concentration."),
                new Card("Têtard Évolutif", CardRarity.Common, "/Images/Cards/communes/Tetard.png","Sois patient avec toi-même, la concentration s'améliore avec la pratique."),
                new Card("Tortue Sage", CardRarity.Rare, "/Images/Cards/communes/Tortue.png","Avance lentement mais sûrement, la régularité est la clé du progrès."),
                new Card("Vache Tranquille", CardRarity.Common, "/Images/Cards/communes/vache.png","Prends le temps de savourer tes réussites, même les plus petites, pour renforcer ta motivation."),
            };
            ListCaisse = GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }

        public static List<Card> GetCaisse2Cards()
        {
            List<Card> ListCaisse = new List<Card>
            {
                // rares
                new Card("Anne", CardRarity.Epic, "/Images/Cards/rares/anne rare.png","?"),
                new Card("Cafard", CardRarity.Epic, "/Images/Cards/rares/cafard rare.png","?"),
                new Card("Cameleon", CardRarity.Epic, "/Images/Cards/rares/cameleon rare.png","?"),
                new Card("Cuisinier", CardRarity.Epic, "/Images/Cards/rares/cuisinier rare.png","?"),
                new Card("Ecureuil", CardRarity.Epic, "/Images/Cards/rares/ecureil rare.png","?"),
                new Card("Flaman Rose", CardRarity.Epic, "/Images/Cards/rares/flaman rose rare.png","?"),
                new Card("Koala", CardRarity.Epic, "/Images/Cards/rares/koala rare.png","?"),
                new Card("Mouche", CardRarity.Epic, "/Images/Cards/rares/mouche rare.png","?"),
                new Card("Panda", CardRarity.Epic, "/Images/Cards/rares/panda rare.png","?"),
                new Card("Verre de terre", CardRarity.Epic, "/Images/Cards/rares/verre de terre rare.png","?"),
                new Card("Zebre", CardRarity.Epic, "/Images/Cards/rares/zebre rare.png","?"),
            };
            ListCaisse = GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }

        public static List<Card> GetCaisse3Cards()
        {
            List<Card> ListCaisse = new List<Card>
            {
                new Card("Hippopotame", CardRarity.Epic, "/Images/Cards/epics/hipo epic.png","?"),
                new Card("Aigle", CardRarity.Epic, "/Images/Cards/epics/aigle EPIC.png","?"),
                new Card("Hibou", CardRarity.Epic, "/Images/Cards/epics/hibou EPIC.png","?"),
                new Card("Elephant", CardRarity.Epic, "/Images/Cards/epics/elephant epic.png","?"),
                new Card("Lezard", CardRarity.Epic, "/Images/Cards/epics/lezard epic.png","?"),
                new Card("Pelican", CardRarity.Epic, "/Images/Cards/epics/pelican EPIC.png","?"),
            };
            ListCaisse=GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }
        public static List<Card> GetCaisse4Cards()
        {
            List<Card> ListCaisse = new List<Card>
            {

            };
            ListCaisse = GetSortedByRarity(ListCaisse);
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