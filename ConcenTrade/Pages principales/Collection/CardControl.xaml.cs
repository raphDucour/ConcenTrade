using System.Windows.Controls;
using System.Windows.Media;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales.Collection
{
    public partial class CardControl : UserControl
    {
        private Card _card;
        private const int STACK_OFFSET_VERTICAL = 40;   // Gros décalage vertical en pixels
        private const int STACK_OFFSET_HORIZONTAL = 25;  // Gros décalage horizontal en pixels

        public CardControl()
        {
            InitializeComponent();
        }

        public void SetCard(Card card)
        {
            _card = card;
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

        public void AddStackedCards(int count)
        {
            // On supprime d'abord toutes les cartes empilées existantes sauf la principale
            for (int i = MainGrid.Children.Count - 1; i > 0; i--)
            {
                MainGrid.Children.RemoveAt(i);
            }

            // Position de base de la carte principale (au-dessus, sans décalage)
            Canvas.SetLeft(CardBorder, 0);
            Canvas.SetTop(CardBorder, 0);
            Panel.SetZIndex(CardBorder, count); // Met la carte principale au-dessus

            // On ajoute les cartes empilées (simples, sans design)
            for (int i = 0; i < count; i++)
            {
                var stackedCard = new Border
                {
                    Width = 200,
                    Height = 280,
                    Background = CardBorder.Background,
                    BorderThickness = new System.Windows.Thickness(2),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    CornerRadius = new System.Windows.CornerRadius(15)
                };

                // Positionner la carte empilée derrière avec décalage
                Canvas.SetLeft(stackedCard, (i + 1) * STACK_OFFSET_HORIZONTAL);
                Canvas.SetTop(stackedCard, (i + 1) * STACK_OFFSET_VERTICAL);
                Panel.SetZIndex(stackedCard, count - i - 1);

                MainGrid.Children.Add(stackedCard);
            }
        }
    }
} 