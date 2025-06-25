using System.Windows;
using System.Windows.Controls;
using Concentrade.Properties;
using Concentrade.Pages_principales.Collection;
using Concentrade.Collections_de_cartes;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System;
using System.Windows.Media.Effects;

namespace Concentrade.Pages_principales
{
    public partial class CollectionPage : Page
    {
        List<Card> cards = Card.GetAllCardsSortedByRarity();
        public bool Caisse2Openable;
        public bool Caisse3Openable;
        public bool Caisse4Openable;



        public CollectionPage()
        {
            InitializeComponent();
            InitializeCaisseOpenable();
            UpdateCaisseLocks();
            InitializeCards();
        }
        

        private void InitializeCards()
        {
            

            foreach (Card card in cards) // Ne prend qu'une carte de chaque type.;
            {
                var cardControl = new CardControl();
                cardControl.SetCard(card);
                
                CardsPanel.Children.Add(cardControl);
            }
        }
        private void InitializeCaisseOpenable()
        {
            int nbCommunes = cards.Count(c => c.Rarity == CardRarity.Common);
            int nbRares = cards.Count(c => c.Rarity == CardRarity.Rare);
            int nbEpics = cards.Count(c => c.Rarity == CardRarity.Epic);
            Caisse2Openable = nbCommunes >= 1;
            Caisse3Openable = nbCommunes >= 20 && nbRares >= 2;
            Caisse4Openable = nbEpics >= 3; // Exemple de condition, à adapter selon ta logique
        }
        private void UpdateCaisseLocks()
        {
            if (!Caisse2Openable)
            {
                Caisse2Cost.Text = "🔒";
                Caisse2Cost.FontSize = 36;
                Caisse2Cost.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xD7, 0x00)); // Doré
                Caisse2Button.IsEnabled = false;
            }
            else
            {
                Caisse2Cost.Text = "Coût: 300 Points";
                Caisse2Cost.FontSize = 14;
                Caisse2Cost.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
                Caisse2Button.IsEnabled = true;
            }
            if (!Caisse3Openable)
            {
                Caisse3Cost.Text = "🔒";
                Caisse3Cost.FontSize = 36;
                Caisse3Cost.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xD7, 0x00));
                Caisse3Button.IsEnabled = false;
            }
            else
            {
                Caisse3Cost.Text = "Coût: 800 Points";
                Caisse3Cost.FontSize = 14;
                Caisse3Cost.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
                Caisse3Button.IsEnabled = true;
            }
            if (!Caisse4Openable)
            {
                Caisse4Cost.Text = "🔒";
                Caisse4Cost.FontSize = 36;
                Caisse4Cost.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xD7, 0x00));
                Caisse4Button.IsEnabled = false;
            }
            else
            {
                Caisse4Cost.Text = "Coût: 1500 Points";
                Caisse4Cost.FontSize = 14;
                Caisse4Cost.Foreground = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));
                Caisse4Button.IsEnabled = true;
            }
        }

        


        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MenuPage());
        }

        private void AcheterButton_Click(object sender, RoutedEventArgs e)
        {
            AchatOverlay.Visibility = Visibility.Visible;
        }

        private void CloseAchatOverlay_Click(object sender, RoutedEventArgs e)
        {
            AchatOverlay.Visibility = Visibility.Collapsed;
        }

        //boutique
        private void BuyCat_Click(object sender, RoutedEventArgs e)
        {
            List<Card> CaisseCards = Card.GetCaisse1Cards();
            if (HasAllCards(CaisseCards))
            {
                MessageBox.Show("Tu possèdes déjà toutes les cartes de cette caisse !");
                return;
            }
            this.NavigationService?.Navigate(new Caisse(CaisseCards,100)); // Caisse Poules
        }

        private void BuyDog_Click(object sender, RoutedEventArgs e)
        {
            List<Card> CaisseCards = Card.GetCaisse2Cards();
            if (HasAllCards(CaisseCards))
            {
                MessageBox.Show("Tu possèdes déjà toutes les cartes de cette caisse !");
                return;
            }
            this.NavigationService?.Navigate(new Caisse(CaisseCards,300));
        }

        private void BuyDragon_Click(object sender, RoutedEventArgs e)
        {
            List<Card> CaisseCards = Card.GetCaisse3Cards();
            if (HasAllCards(CaisseCards))
            {
                MessageBox.Show("Tu possèdes déjà toutes les cartes de cette caisse !");
                return;
            }
            this.NavigationService?.Navigate(new Caisse(CaisseCards,800));
        }

        private void BuyCaisse4_Click(object sender, RoutedEventArgs e)
        {
            List<Card> CaisseCards = Card.GetCaisse4Cards();
            if (HasAllCards(CaisseCards))
            {
                MessageBox.Show("Tu possèdes déjà toutes les cartes de cette caisse !");
                return;
            }
            this.NavigationService?.Navigate(new Caisse(CaisseCards, 2000));
        }

        private bool HasAllCards(List<Card> caisseCards)
        {
            return caisseCards.All(c => cards.Any(u => u.Name == c.Name));
        }


        //particules
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                CreateAndAnimateParticles(10);
            }
        }        
        
        
        private Random _random = new Random();
        private void CreateAndAnimateParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Ellipse particle = new Ellipse
                {
                    Fill = new SolidColorBrush(Colors.White),
                    Effect = new BlurEffect()
                };

                double size = _random.Next(5, 40);
                particle.Width = size;
                particle.Height = size;
                particle.Opacity = _random.NextDouble() * 0.4 + 0.1;
                ((BlurEffect)particle.Effect).Radius = _random.Next(5, 15);

                particle.RenderTransform = new TranslateTransform(_random.Next(0, (int)this.ActualWidth), _random.Next(0, (int)this.ActualHeight));

                ParticleCanvas.Children.Add(particle);
                AnimateParticle(particle);
            }
        }

        private void AnimateParticle(Ellipse particle)
        {
            var transform = (TranslateTransform)particle.RenderTransform;
            if (this.ActualWidth == 0 || this.ActualHeight == 0) return;

            double endX = _random.NextDouble() > 0.5 ? this.ActualWidth + 100 : -100;
            double endY = _random.Next(0, (int)this.ActualHeight);

            var animX = new DoubleAnimation
            {
                To = endX,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)),
            };

            var animY = new DoubleAnimation
            {
                To = endY,
                Duration = TimeSpan.FromSeconds(_random.Next(20, 60)),
            };

            animX.Completed += (s, e) =>
            {
                if (this.ActualWidth > 0 && this.ActualHeight > 0)
                {
                    transform.X = _random.NextDouble() > 0.5 ? -50 : this.ActualWidth + 50;
                    transform.Y = _random.Next(0, (int)this.ActualHeight);
                    AnimateParticle(particle);
                }
            };

            transform.BeginAnimation(TranslateTransform.XProperty, animX);
            transform.BeginAnimation(TranslateTransform.YProperty, animY);
        }     
    }
} 