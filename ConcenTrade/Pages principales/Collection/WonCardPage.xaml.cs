using System.Windows;
using System.Windows.Controls;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales.Collection
{
    public partial class WonCardPage : Page
    {
        public WonCardPage(Card card)
        {
            InitializeComponent();
            CardControlFull.SetCard(card);
            CardControlFull.AnimateTipAppearance();
        }

        private void VoirCollection_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new CollectionPage());
        }
    }
} 