using System.Windows;
using System.Windows.Controls;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales.Collection
{
    public partial class WonCardPage : Page
    {
        // Initialise la page avec la carte gagnée et anime son affichage
        public WonCardPage(Card card)
        {
            InitializeComponent();
            CardControlFull.SetCard(card);
            CardControlFull.AnimateTipAppearance();
        }

        // Navigue vers la page de collection
        private void VoirCollection_Click(object sender, RoutedEventArgs e)
        {
            UserManager.PushIntoBDD_FireAndForget();
            this.NavigationService?.Navigate(new CollectionPage());
        }
    }
} 