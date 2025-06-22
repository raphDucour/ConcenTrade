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

        public Caisse(List<Card> numCaisse)
        {
            InitializeComponent();

            _possibleCards = numCaisse;
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
            if (_possibleCards == null || _possibleCards.Count == 0)
                throw new InvalidOperationException("La liste des cartes possibles est vide.");

            int index = _random.Next(_possibleCards.Count);
            return _possibleCards[index];
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
                            _possibleCards.Remove(wonCard);
                            DisplayPossibleCards();
                            this.NavigationService?.Navigate(new WonCardPage(wonCard));
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