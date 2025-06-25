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
                new Card("Chat Zen", CardRarity.Common, "/Images/Cards/communes/Chat.png","Lis quelques pages avant de commencer : ça calme l’esprit."),
                new Card("Chien Focus", CardRarity.Common, "/Images/Cards/communes/Chien.png","Commence ta session par 1 minute de respiration."),
                new Card("Coq Matinal", CardRarity.Common, "/Images/Cards/communes/Poule.png","Commence par la tâche la plus simple pour te lancer."),
                new Card("Renard Sage", CardRarity.Common, "/Images/Cards/communes/renard.png","Commence lentement, mais avec intention : la constance paie."),
                new Card("Cochon Calme", CardRarity.Common, "/Images/Cards/communes/cochon.png","Écris tes objectifs avant de te lancer, ça clarifie l’esprit."),
                new Card("Dauphin Agile", CardRarity.Common, "/Images/Cards/communes/Dophin.png","Respire profondément avant de commencer, comme un dauphin calme."),
                new Card("Fourmi Ouvrière", CardRarity.Common, "/Images/Cards/communes/fourmi.png","Avance petit à petit, la régularité fait la force."),
                new Card("Girafe Sereine", CardRarity.Common, "/Images/Cards/communes/Giraffe.png","Prends de la hauteur pour mieux organiser tes idées."),
                new Card("Phoque Paisible", CardRarity.Common, "/Images/Cards/communes/phoc.png","Travaille en douceur, évite la précipitation inutile."),
                new Card("Rat Rusé", CardRarity.Common, "/Images/Cards/communes/rat.png","Sois précis et rapide : chaque mouvement compte vraiment."),
                new Card("Singe Joueur", CardRarity.Common, "/Images/Cards/communes/singe.png","Ce singe est distrait, évite ça, reste focus sur ta tâche."),
                new Card("Taupe Travailleuse", CardRarity.Common, "/Images/Cards/communes/taupe.png","Focus comme la taupe, observe chaque détail."),
                new Card("Têtard Évolutif", CardRarity.Common, "/Images/Cards/communes/Tetard.png","Sois patient avec toi-même, la concentration s'améliore avec la pratique."),
                new Card("Tortue Sage", CardRarity.Common, "/Images/Cards/communes/Tortue.png","Avance lentement mais sûrement, la régularité est la clé du progrès."),
                new Card("Vache LockedIn", CardRarity.Common, "/Images/Cards/communes/vache.png","Garde une concentration intense, rien ne te distrait."),

                // rares
                new Card("Âne Concentré", CardRarity.Rare, "/Images/Cards/rares/anne rare.png","Cet âne est distrait... ne sois pas un âne."),
                new Card("Cafard Chillax", CardRarity.Rare, "/Images/Cards/rares/cafard rare.png","Accorde-toi de vraies pauses pour mieux repartir."),
                new Card("Caméléon Focus", CardRarity.Rare, "/Images/Cards/rares/cameleon rare.png","Adapte ton environnement à ta tâche."),
                new Card("Chef Ornitho", CardRarity.Rare, "/Images/Cards/rares/cuisinier rare.png","La concentration, c'est comme une cuisson: à feu doux mais constant."),
                new Card("Professeur Noisette", CardRarity.Rare, "/Images/Cards/rares/ecureuil rare.png","Crée-toi un coin calme, rien qu'à toi."),
                new Card("Flamàn d'Oz", CardRarity.Rare, "/Images/Cards/rares/flaman rose rare.png","Adopte une posture royale."),
                new Card("Koala Cool", CardRarity.Rare, "/Images/Cards/rares/koala rare.png","Adopte la lenteur consciente : prends des pauses régulières."),
                new Card("Œil de Mouche", CardRarity.Rare, "/Images/Cards/rares/mouche rare.png","Élimine les bourdonnements (avec un insecticide)."),
                new Card("Mélodie du Panda", CardRarity.Rare, "/Images/Cards/rares/panda.png","Laisse-toi porter par le rythme de ta respiration."),
                new Card("Ver de Livre", CardRarity.Rare, "/Images/Cards/rares/verre de terrre rare.png","Plonge dans ta lecture comme ce ver dans son livre."),
                new Card("Zêrbe", CardRarity.Rare, "/Images/Cards/rares/zebre rare.png","Chill comme ce Zêrbe."),

                // epics
                new Card("Hippo Boss", CardRarity.Epic, "/Images/Cards/epics/hipo epic.png","Lunettes en place, stylos prêts, focus activé."),
                new Card("R-Aigle", CardRarity.Epic, "/Images/Cards/epics/aigle epic.png ","Vision de roi, costar taillé, objectif verrouillé, réussite assurée."),
                new Card("Hibourge", CardRarity.Epic, "/Images/Cards/epics/hibou epic.png ","Pose ton tel, concentre-toi, les messages attendront."),
                new Card("Elegang", CardRarity.Epic, "/Images/Cards/epics/elephant epic.png","Reste focus, ta route dépend de toi."),
                new Card("Lez", CardRarity.Epic, "/Images/Cards/epics/lezard epic.png","Change de posture souvent, ton cerveau te dira merci."),
                new Card("PeliCalme", CardRarity.Epic, "/Images/Cards/epics/pelican epic.png","Planifie ta journée, évite les distractions numériques."),

                // legendaires
                new Card("Celesthar", CardRarity.Legendary, "/Images/Cards/legendaires/ange legendaire.png","?"),
                new Card("AntaGoat", CardRarity.Legendary, "/Images/Cards/legendaires/chevre legendaire.png","?"),
                new Card("Vincent", CardRarity.Legendary, "/Images/Cards/legendaires/dragon legendaire.png","?"),
                new Card("Pwin Tu", CardRarity.Legendary, "/Images/Cards/legendaires/grosnez legendaire.png","?"),
                new Card("Garou", CardRarity.Legendary, "/Images/Cards/legendaires/loup legendaire.png","?"),

            };
        }

        // NOTE : Les nouvelles cartes ne sont pas encore dans les caisses.
        // Vous devrez les ajouter manuellement dans les méthodes GetCaisse ci-dessous.

        public static List<Card> GetCaisse1Cards()
        {
            List < Card >  ListCaisse = new List<Card>
            {
                // Communes
                new Card("Chat Zen", CardRarity.Common, "/Images/Cards/communes/Chat.png","?"),
                new Card("Chien Focus", CardRarity.Common, "/Images/Cards/communes/Chien.png","?"),
                new Card("Coq Matinal", CardRarity.Common, "/Images/Cards/communes/Poule.png","?"),
                new Card("Renard Sage", CardRarity.Common, "/Images/Cards/communes/renard.png","?"),
                new Card("Cochon Calme", CardRarity.Common, "/Images/Cards/communes/cochon.png","?"),
                new Card("Dauphin Agile", CardRarity.Common, "/Images/Cards/communes/Dophin.png","?"),
                new Card("Fourmi Ouvrière", CardRarity.Common, "/Images/Cards/communes/fourmi.png","?"),
                new Card("Girafe Sereine", CardRarity.Common, "/Images/Cards/communes/Giraffe.png","?"),
                new Card("Phoque Paisible", CardRarity.Common, "/Images/Cards/communes/phoc.png","?"),
                new Card("Rat Rusé", CardRarity.Common, "/Images/Cards/communes/rat.png","?"),
                new Card("Singe Joueur", CardRarity.Common, "/Images/Cards/communes/singe.png","?"),
                new Card("Taupe Travailleuse", CardRarity.Common, "/Images/Cards/communes/taupe.png","?"),
                new Card("Têtard Évolutif", CardRarity.Common, "/Images/Cards/communes/Tetard.png","?"),
                new Card("Tortue Sage", CardRarity.Common, "/Images/Cards/communes/Tortue.png","?"),
                new Card("Vache LockedIn", CardRarity.Common, "/Images/Cards/communes/vache.png","?"),
            };
            ListCaisse = GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }

        public static List<Card> GetCaisse2Cards()
        {
            List<Card> ListCaisse = new List<Card>
            {
                // rares
                new Card("Âne Concentré", CardRarity.Rare, "/Images/Cards/rares/anne rare.png","?"),
                new Card("Cafard Chillax", CardRarity.Rare, "/Images/Cards/rares/cafard rare.png","?"),
                new Card("Caméléon Focus", CardRarity.Rare, "/Images/Cards/rares/cameleon rare.png","?"),
                new Card("Chef Ornitho", CardRarity.Rare, "/Images/Cards/rares/cuisinier rare.png","?"),
                new Card("Professeur Noisette", CardRarity.Rare, "/Images/Cards/rares/ecureuil rare.png","?"),
                new Card("Flamàn d'Oz", CardRarity.Rare, "/Images/Cards/rares/flaman rose rare.png","?"),
                new Card("Koala Cool", CardRarity.Rare, "/Images/Cards/rares/koala rare.png","?"),
                new Card("Œil de Mouche", CardRarity.Rare, "/Images/Cards/rares/mouche rare.png","?"),
                new Card("Mélodie du Panda", CardRarity.Rare, "/Images/Cards/rares/panda.png","?"),
                new Card("Ver de Livre", CardRarity.Rare, "/Images/Cards/rares/verre de terrre rare.png","?"),
                new Card("Zêrbe", CardRarity.Rare, "/Images/Cards/rares/zebre rare.png","?"),
            };
            ListCaisse = GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }

        public static List<Card> GetCaisse3Cards()
        {
            List<Card> ListCaisse = new List<Card>
            {
                // epics
                new Card("Hippo Boss", CardRarity.Epic, "/Images/Cards/epics/hipo epic.png","?"),
                new Card("R-Aigle", CardRarity.Epic, "/Images/Cards/epics/aigle epic.png ","?"),
                new Card("Hibourge", CardRarity.Epic, "/Images/Cards/epics/hibou epic.png ","?"),
                new Card("Elegang", CardRarity.Epic, "/Images/Cards/epics/elephant epic.png","?"),
                new Card("Lez", CardRarity.Epic, "/Images/Cards/epics/lezard epic.png","?"),
                new Card("PeliCalme", CardRarity.Epic, "/Images/Cards/epics/pelican epic.png","?"),
            };
            ListCaisse=GetSortedByRarity(ListCaisse);
            return filterByOptainedCards(ListCaisse);
        }
        public static List<Card> GetCaisse4Cards()
        {
            List<Card> ListCaisse = new List<Card>
            {
                // legendaires
                new Card("Celesthar", CardRarity.Legendary, "/Images/Cards/legendaires/ange legendaire.png","?"),
                new Card("AntaGoat", CardRarity.Legendary, "/Images/Cards/legendaires/chevre legendaire.png","?"),
                new Card("Vincent", CardRarity.Legendary, "/Images/Cards/legendaires/dragon legendaire.png","?"),
                new Card("Pwin Tu", CardRarity.Legendary, "/Images/Cards/legendaires/grosnez legendaire.png","?"),
                new Card("Garou", CardRarity.Legendary, "/Images/Cards/legendaires/loup legendaire.png","?"),
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