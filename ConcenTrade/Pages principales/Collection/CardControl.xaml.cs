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

        public void SetCard(Card name)
        {
            _card = name;
            UpdateCardDisplay();
        }

        private void UpdateCardDisplay()
        {
            if (_card != null)
            {
                CardNameText.Text = _card.Name;
                CardBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_card.color));
                IconeText.Text = _card.icone;
            }
        }
    }
} 