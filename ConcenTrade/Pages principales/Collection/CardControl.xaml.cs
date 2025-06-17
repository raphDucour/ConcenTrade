using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales.Collection
{
    public partial class CardControl : UserControl
    {
        private Card? _card;

        // NOUVELLE PROPRIÉTÉ PUBLIQUE
        public Card? CardData => _card;

        private const int STACK_OFFSET_VERTICAL = 8;
        private const int STACK_OFFSET_HORIZONTAL = 6;
        private const int MAX_STACKED_CARDS = 5;

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

                if (!string.IsNullOrEmpty(_card.IconPath))
                {
                    try
                    {
                        CardIcon.Source = new BitmapImage(new Uri($"pack://application:,,,{_card.IconPath}", UriKind.Absolute));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                        CardIcon.Source = null;
                    }
                }
            }
        }

        public CardControl Copy()
        {
            string savedXaml = XamlWriter.Save(this);
            string fixedXaml = savedXaml.Replace(" Name=\"MainGrid\"", "");
            return (CardControl)XamlReader.Parse(fixedXaml);
        }
    }
}