using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Concentrade.Collections_de_cartes;

namespace Concentrade.Pages_principales.Collection
{
    public partial class Caisse : Page
    {
        private readonly List<Card> _possibleCards;
        private bool _isSpinning = false;
        private static readonly Random _random = new Random();

        public Caisse(int numCaisse)
        {
            InitializeComponent();

            _possibleCards = numCaisse switch
            {
                1 => Card.GetCaisse1Cards(),
                2 => Card.GetCaisse2Cards(),
                3 => Card.GetCaisse3Cards(),
                _ => Card.GetCaisse1Cards()
            };

            DisplayPossibleCards();
            InitializeRoulletteCards();
        }

        private void DisplayPossibleCards()
        {
            CardsPanel.Children.Clear();
            foreach (var cardData in _possibleCards)
            {
                var cardControl = new CardControl();
                cardControl.SetCard(cardData);
                CardsPanel.Children.Add(cardControl);
            }
        }

        private void InitializeRoulletteCards()
        {
            RoulettePanel.Children.Clear();
            for (int i = 0; i < 100; i++)
            {
                Card randomCardData = GetRandomCardFromPossible();
                var newControl = new CardControl
                {
                    Width = 150,
                    Margin = new Thickness(40)
                };
                newControl.SetCard(randomCardData);
                Canvas.SetLeft(newControl, i * 230);
                RoulettePanel.Children.Add(newControl);
            }
        }

        private Card GetRandomCardFromPossible()
        {
            double probability = _random.NextDouble();
            if (probability < 0.5) return _possibleCards[0];
            if (probability < 0.9) return _possibleCards.Count > 1 ? _possibleCards[1] : _possibleCards[0];
            return _possibleCards.Count > 2 ? _possibleCards[2] : _possibleCards[0];
        }

        private void BtnAcheter_Click(object sender, RoutedEventArgs e)
        {
            if (_isSpinning) return;
            _isSpinning = true;
            BtnAcheter.IsEnabled = false;
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
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            // Le code de gestion de la fin de l'animation est restauré et amélioré ici
            animation.Completed += (s, e) =>
            {
                _isSpinning = false;
                BtnAcheter.IsEnabled = true;

                // On calcule quelle carte est au milieu de l'écran à la fin de l'animation
                double finalOffset = -15000;
                double centerPosition = -finalOffset + (RouletteContainer.ActualWidth / 2);
                int winningIndex = (int)(centerPosition / 230); // 230 = Largeur de la carte + Marge

                if (winningIndex >= 0 && winningIndex < RoulettePanel.Children.Count)
                {
                    // On récupère le contrôle graphique de la carte gagnante
                    if (RoulettePanel.Children[winningIndex] is CardControl winningControl)
                    {
                        // On accède aux données de la carte via notre nouvelle propriété CardData
                        if (winningControl.CardData is Card wonCard)
                        {
                            Card.AddCard(wonCard); // On ajoute la carte à la collection
                            MessageBox.Show($"Félicitations ! Vous avez obtenu : {wonCard.Name}", "Nouvelle Carte !");
                        }
                    }
                }
            };

            ScrollTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.Navigate(new CollectionPage());
            }
        }
    }
}