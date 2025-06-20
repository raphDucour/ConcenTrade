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
       

        private int userPoints;

        public CollectionPage()
        {
            InitializeComponent();
            LoadUserPoints();
            InitializeCards();
        }

        private void InitializeCards()
        {
            

            foreach (Card card in cards) // Ne prend qu'une carte de chaque type
            {
                var cardControl = new CardControl();
                cardControl.SetCard(card);
                
                CardsPanel.Children.Add(cardControl);
            }
        }

        private void LoadUserPoints()
        {
            userPoints = 10000; //Settings.Default.Points;
        }

        private void SaveUserPoints()
        {
            Settings.Default.Points = userPoints;
            Settings.Default.Save();
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

        private bool TryBuyCard(Card cardName, int cost)
        {
            if (userPoints >= cost)
            {
                userPoints -= cost;
                SaveUserPoints();
                Card.AddCard(cardName);
                
                // Mettre à jour le tableau avant de rafraîchir
                cards = Card.GetAllCardsSortedByRarity();
                
                CardsPanel.Children.Clear();
                InitializeCards();
                
                MessageBox.Show($"{cardName} acheté !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                MessageBox.Show("Points insuffisants !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void BuyCat_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new Caisse(1)); // Caisse Poules
        }

        private void BuyDog_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new Caisse(2)); // Caisse QoC
        }

        private void BuyDragon_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new Caisse(3)); // Caisse Dragon
        }
        private Random _random = new Random();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                CreateAndAnimateParticles(10);
            }
        }

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