using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Concentrade.Collections_de_cartes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace Concentrade.Pages_principales.Collection
{
    public partial class Caisse : Page
    {
        private List<Card> Cards { get; set; }
        private bool isSpinning = false;
        private DispatcherTimer scrollTimer;
        private double scrollPosition = 0;
        private static readonly Random random = new Random();

        public Caisse(int numCaisse)
        {
            InitializeComponent();
            
            // Sélection de la caisse appropriée
            Cards = numCaisse switch
            {
                1 => Card.GetCaisse1Cards(), // Caisse Poules
                2 => Card.GetCaisse2Cards(), // Caisse QoC
                3 => Card.GetCaisse3Cards(), // Caisse Dragon
                _ => Card.GetCaisse1Cards() // Par défaut, retourne la caisse Poules
            };
            
            
            InitializeCards();
        }


        private void InitializeCards()
        {
            InitializeRoulletteCards();
            foreach (Card card in Cards)
            {
                var cardControl = new CardControl();
                cardControl.SetCard(card);
                CardsPanel.Children.Add(cardControl);
            }
        }
        private void InitializeRoulletteCards()
        {
            RoulettePanel.Children.Clear();
            for (int i = 0; i < 100; i++)
            {
                Card card = getRandomCardFromProbability(Cards);
                var cardControl = new CardControl();
                cardControl.Width = 150; // Largeur fixe pour chaque carte
                cardControl.Margin = new Thickness(40); // Espacement entre les cartes
                cardControl.SetCard(card);
                Canvas.SetLeft(cardControl, i * 230); // 150 (largeur) + 2*40 (marge)
                RoulettePanel.Children.Add(cardControl);
            }
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            // Retour à la page précédente
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
            }
            
        }

        private void BtnAcheter_Click(object sender, RoutedEventArgs e)
        {
            if (isSpinning) return;
            isSpinning = true;
            BtnAcheter.IsEnabled = false;

            // Démarrer l'animation
            StartRoulette();
        }

        

        private void StartRoulette()
        {
            InitializeRoulletteCards();
            var animation = new DoubleAnimation
            {
                From = 0,
                To = -15000,
                Duration = TimeSpan.FromSeconds(10),
                RepeatBehavior = new RepeatBehavior(1),
                EasingFunction = new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            animation.Completed += (s, e) =>
            {
                isSpinning = false;
                BtnAcheter.IsEnabled = true;


                double finalOffset = -15000; // valeur de "To" dans l'animation
                double centerPosition = -finalOffset + (RouletteContainer.ActualWidth / 2);
                int visibleIndex = (int)(centerPosition / 230);

                if (visibleIndex >= 0 && visibleIndex < RoulettePanel.Children.Count)
                {
                    // Étape 2 : Récupération du contrôle de carte
                    var selectedCardControl = RoulettePanel.Children[visibleIndex] as CardControl;

                    if (selectedCardControl != null)
                    {
                        // Étape 3 : Ajouter la carte à la collection du joueur
                        var selectedCardField = typeof(CardControl)
                            .GetField("_card", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var selectedCard = selectedCardField?.GetValue(selectedCardControl) as Card;

                        if (selectedCard != null)
                        {
                            Card.AddCard(selectedCard);

                            // Facultatif : Afficher un message ou feedback visuel
                            MessageBox.Show($"Tu as gagné la carte : {selectedCard.Name} !");
                        }
                    }
                }
            };

            ScrollTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private Card getRandomCardFromProbability(List<Card> cards)
        {
            double probability = random.NextDouble();
            return probability switch
            {
                < 0.5 => cards[0],
                < 0.9 => cards[1],
                _     => cards[2]
            };
        }


    }
} 