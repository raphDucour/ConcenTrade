using System.Windows.Controls;
using System.Windows.Media.Effects;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales.Collection
{
    public partial class CardControl : UserControl
    {
        private bool isOwned = false;

        public CardControl()
        {
            InitializeComponent();
        }

        public void SetCardName(string name)
        {
            if (CardNameText != null)
            {
                CardNameText.Text = name;
                CardEmoji.Text = GetCardEmoji(name);
                isOwned = Card.GetAllCards().Exists(c => c.Name == name);
                UpdateCardAppearance();
            }
        }

        private string GetCardEmoji(string cardName)
        {
            switch (cardName)
            {
                case "Chat Zen": return "🐱";
                case "Chien Focus": return "🐕";
                case "Panda Méditant": return "🐼";
                case "Renard Sage": return "🦊";
                case "Lapin Paisible": return "🐰";
                case "Loup Alpha": return "🐺";
                case "Coq Matinal": return "🐓";
                case "Paon Majestueux": return "🦚";
                case "Dragon Ancestral": return "🐉";
                default: return "❓";
            }
        }

        private void UpdateCardAppearance()
        {
            if (!isOwned)
            {
                CardBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 42, 42));
                CardGlow.BlurRadius = 0;
                CardGlow.Opacity = 0;
                CardEmoji.Opacity = 0.5;
                return;
            }

            var card = Card.GetAllCards().Find(c => c.Name == CardNameText.Text);
            if (card == null) return;

            CardEmoji.Opacity = 1.0;

            switch (card.Rarity)
            {
                case "Commun":
                    CardBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(43, 76, 140));
                    CardGlow.Color = System.Windows.Media.Colors.White;
                    CardGlow.BlurRadius = 5;
                    CardGlow.Opacity = 0.3;
                    break;
                case "Rare":
                    CardBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(109, 40, 217));
                    CardGlow.Color = System.Windows.Media.Colors.Purple;
                    CardGlow.BlurRadius = 10;
                    CardGlow.Opacity = 0.4;
                    break;
                case "Épique":
                    CardBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(194, 65, 12));
                    CardGlow.Color = System.Windows.Media.Colors.Orange;
                    CardGlow.BlurRadius = 15;
                    CardGlow.Opacity = 0.5;
                    break;
                case "Légendaire":
                    CardBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(161, 98, 7));
                    CardGlow.Color = System.Windows.Media.Colors.Gold;
                    CardGlow.BlurRadius = 20;
                    CardGlow.Opacity = 0.6;
                    break;
            }
        }
    }
} 