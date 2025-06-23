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
        public Caisse(List<Card> numCaisse, int prix)
        {
            InitializeComponent();

            _possibleCards = numCaisse;
            _price = prix;
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
            if (Properties.Settings.Default.Points < _price) return;
            if (_isSpinning) return;
            _isSpinning = true;
            BtnAcheter.IsEnabled = false;
            StartRoulette();
            Properties.Settings.Default.Points -= _price; // Déduit le prix de la caisse    
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
                            // Avant d'ajouter la carte et de naviguer, on cherche la version complète
                            // de la carte dans GetAllPossibleCards() pour avoir la description correcte.
                            Card completeWonCard = Card.GetAllPossibleCards().FirstOrDefault(c => c.Name == wonCard.Name);

                            if (completeWonCard != null)
                            {
                                Card.AddCard(completeWonCard); // On ajoute la carte complète à la collection
                                _possibleCards.Remove(wonCard); // Remove the placeholder card from _possibleCards
                                DisplayPossibleCards();

                                // Puis on navigue vers WonCardPage avec la carte complète
                                this.NavigationService?.Navigate(new WonCardPage(completeWonCard));
                            }
                            else
                            {
                                // Fallback si la carte gagnée n'est pas trouvée dans la liste complète des cartes
                                // Cela ne devrait normalement pas arriver si les noms sont cohérents.
                                System.Diagnostics.Debug.WriteLine($"Erreur : La carte gagnée '{wonCard.Name}' n'a pas été trouvée dans la liste complète des cartes.");
                                Card.AddCard(wonCard); // Add the wonCard as is if the complete one isn't found
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


        private void BtnRetour_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.Navigate(new CollectionPage());
            }
        }
    }
}