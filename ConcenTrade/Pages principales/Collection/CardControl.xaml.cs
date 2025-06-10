using System.Windows.Controls;
using System.Windows.Media;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales.Collection
{
    public partial class CardControl : UserControl
    {
        private Card _card;

        public CardControl()
        {
            InitializeComponent();
        }

        public void SetCard(string name)
        {
            _card = new Card(name);
            UpdateCardDisplay();
        }

        private void UpdateCardDisplay()
        {
            if (_card != null)
            {
                CardNameText.Text = _card.Name;
                CardBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Card.GetRarityColor(_card.Rarity)));
            }
        }
    }
} 