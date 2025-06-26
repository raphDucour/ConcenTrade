using System;
using System.Collections.Generic;
using System.Linq;
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
        private static int _price;

        // Initialise la page de caisse avec les cartes possibles et le prix
        public Caisse(List<Card> numCaisse, int prix)
        {
            InitializeComponent();

            _possibleCards = numCaisse;
            _price = prix;
            DisplayPossibleCards();
            InitializeRoulletteCards();
        }

        // Affiche les cartes possibles dans le panneau
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

        // Initialise les cartes de la roulette avec des cartes aléatoires
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

        // Retourne une carte aléatoire depuis la liste des cartes possibles
        private Card GetRandomCardFromPossible()
        {
            if (_possibleCards == null || _possibleCards.Count == 0)
                throw new InvalidOperationException("La liste des cartes possibles est vide.");

            int index = _random.Next(_possibleCards.Count);
            return _possibleCards[index];
        }

        // Gère le clic sur le bouton d'achat et lance la roulette
        private void BtnAcheter_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Points < _price) return;
            if (_isSpinning) return;
            _isSpinning = true;
            BtnAcheter.IsEnabled = false;
            StartRoulette();
            Properties.Settings.Default.Points -= _price;
        }

        // Lance l'animation de la roulette et détermine la carte gagnante
        private void StartRoulette()
        {
            var animation = new DoubleAnimation
            {
                From = 0,
                To = -7000,
                Duration = TimeSpan.FromSeconds(6),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            animation.Completed += (s, e) =>
            {
                _isSpinning = false;
                BtnAcheter.IsEnabled = true;

                double finalOffset = -15000;
                double centerPosition = -finalOffset + (RouletteContainer.ActualWidth / 2);
                int winningIndex = (int)(centerPosition / 230);

                if (winningIndex >= 0 && winningIndex < RoulettePanel.Children.Count)
                {
                    if (RoulettePanel.Children[winningIndex] is CardControl winningControl)
                    {
                        if (winningControl.CardData is Card wonCard)
                        {
                            Card completeWonCard = Card.GetAllPossibleCards().FirstOrDefault(c => c.Name == wonCard.Name);

                            if (completeWonCard != null)
                            {
                                Card.AddCard(completeWonCard);
                                _possibleCards.Remove(wonCard);
                                DisplayPossibleCards();

                                this.NavigationService?.Navigate(new WonCardPage(completeWonCard));
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Erreur : La carte gagnée '{wonCard.Name}' n'a pas été trouvée dans la liste complète des cartes.");
                                Card.AddCard(wonCard);
                                _possibleCards.Remove(wonCard);
                                DisplayPossibleCards();
                                this.NavigationService?.Navigate(new WonCardPage(wonCard));
                            }
                        }
                    }
                }
            };

            ScrollTransform.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        // Navigue vers la page de collection
        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.Navigate(new CollectionPage());
            }
        }
    }
}