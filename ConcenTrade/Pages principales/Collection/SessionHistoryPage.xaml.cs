using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Concentrade.Pages_principales
{
    public partial class SessionHistoryPage : Page
    {
        private Random _random = new Random();
        private ObservableCollection<StatEntry> stats = new ObservableCollection<StatEntry>();

        // Initialise la page d'historique des sessions
        public SessionHistoryPage()
        {
            InitializeComponent();
            this.Loaded += SessionHistoryPage_Loaded;
            LoadStatsAndChart();
        }

        // Parse une chaîne de données de session en liste de valeurs numériques
        public static List<double> ParseSessionString(string data)
        {
            var result = new List<double>();
            if (string.IsNullOrEmpty(data)) return result;

            var parts = data.Split('*');
            foreach (var part in parts)
            {
                if (double.TryParse(part, out double value))
                {
                    result.Add(value);
                }
            }
            return result;
        }

        // Charge les statistiques et génère le graphique avec des données simulées
        private void LoadStatsAndChart()
        {
            DateTime startDate = DateTime.Today.AddDays(-15);
            var rand = new Random();
            int nbJours = 18;
            double[] pomodoroDurations = { 0, 0.45, 0.75, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 };
            var hoursPerDay = new List<double>();
            for (int i = 0; i < nbJours; i++)
            {
                if (rand.NextDouble() < 0.2)
                    hoursPerDay.Add(0);
                else
                    hoursPerDay.Add(pomodoroDurations[rand.Next(1, pomodoroDurations.Length)]);
            }
            var dates = Enumerable.Range(0, nbJours)
                                  .Select(i => startDate.AddDays(i).ToString("dd/MM"))
                                  .ToArray();
            stats = new ObservableCollection<StatEntry>(dates.Zip(hoursPerDay, (date, hours) => new StatEntry { Date = date, Hours = hours })
                                                            .Reverse());
            StatsDataGrid.ItemsSource = stats;
            UpdateChartFromStats();
            UpdateStatsBlock();
        }

        // Met à jour le graphique avec les données des statistiques
        private void UpdateChartFromStats()
        {
            var values = new LiveCharts.ChartValues<double>(stats.Select(s => s.Hours));
            var labels = stats.Select(s => s.Date).ToArray();
            LineChart.Series = new LiveCharts.SeriesCollection
            {
                new LiveCharts.Wpf.LineSeries
                {
                    Title = "Heures de travail",
                    Values = values,
                    PointGeometry = LiveCharts.Wpf.DefaultGeometries.Circle,
                    PointGeometrySize = 10,
                    StrokeThickness = 3,
                    Fill = Brushes.Transparent,
                    Stroke = (Brush)App.Current.Resources["ZenButtonBorderBrush"],
                    Foreground = (Brush)App.Current.Resources["ZenTextBrush"]
                }
            };
            LineChart.AxisX.Clear();
            LineChart.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Date",
                Labels = labels,
                Foreground = (Brush)App.Current.Resources["ZenTextBrush"],
                Separator = new LiveCharts.Wpf.Separator { Step = 1, IsEnabled = false },
                FontSize = 16
            });
            LineChart.AxisY.Clear();
            LineChart.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Heures de concentration",
                MinValue = 0,
                Foreground = (Brush)App.Current.Resources["ZenTextBrush"],
                Separator = new LiveCharts.Wpf.Separator { Step = 1, IsEnabled = false },
                FontSize = 16
            });
        }

        // Met à jour le bloc des statistiques avec les valeurs calculées
        private void UpdateStatsBlock()
        {
            if (stats.Count == 0) return;
            var values = stats.Select(s => s.Hours).ToList();
            double moyenne = values.Average();
            double max = values.Max();
            double total = values.Sum();
            StatMoyenne.Text = moyenne.ToString("0.00") + " h";
            StatMax.Text = max.ToString("0.00") + " h";
            StatTotal.Text = total.ToString("0.00") + " h";
        }

        public class StatEntry
        {
            public string Date { get; set; } = "";
            public double Hours { get; set; }
        }

        // Navigue vers la page de menu
        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new MenuPage());
        }

        // Crée et anime les particules lors du chargement de la page
        private void SessionHistoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.ActualWidth > 0 && this.ActualHeight > 0)
            {
                CreateAndAnimateParticles(10);
            }
        }

        // Crée et anime un nombre spécifique de particules
        private void CreateAndAnimateParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Ellipse particle = new Ellipse
                {
                    Fill = new SolidColorBrush(Colors.White),
                    Effect = new System.Windows.Media.Effects.BlurEffect()
                };

                double size = _random.Next(5, 40);
                particle.Width = size;
                particle.Height = size;
                particle.Opacity = _random.NextDouble() * 0.4 + 0.1;
                ((System.Windows.Media.Effects.BlurEffect)particle.Effect).Radius = _random.Next(5, 15);

                particle.RenderTransform = new TranslateTransform(_random.Next(0, (int)this.ActualWidth), _random.Next(0, (int)this.ActualHeight));

                ParticleCanvas.Children.Add(particle);
                AnimateParticle(particle);
            }
        }

        // Anime une particule individuelle avec un mouvement aléatoire
        private void AnimateParticle(Ellipse particle)
        {
            var transform = (TranslateTransform)particle.RenderTransform;

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

        // Affiche le panneau graphique et masque le tableau
        private void BtnGraphique_Click(object sender, RoutedEventArgs e)
        {
            SetPanelVisibility(true);
        }

        // Affiche le panneau tableau et masque le graphique
        private void BtnTableau_Click(object sender, RoutedEventArgs e)
        {
            SetPanelVisibility(false);
        }

        // Gère la visibilité des panneaux graphique et tableau
        private void SetPanelVisibility(bool showGraph)
        {
            GraphiquePanel.Visibility = showGraph ? Visibility.Visible : Visibility.Collapsed;
            TableauPanel.Visibility = showGraph ? Visibility.Collapsed : Visibility.Visible;
            if (showGraph)
            {
                BtnGraphique.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700"));
                BtnGraphique.Foreground = Brushes.Black;
                BtnGraphique.FontWeight = FontWeights.Bold;
                BtnTableau.Background = Brushes.Transparent;
                BtnTableau.Foreground = (Brush)App.Current.Resources["ZenButtonTextBrush"];
                BtnTableau.FontWeight = FontWeights.Normal;
            }
            else
            {
                BtnTableau.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700"));
                BtnTableau.Foreground = Brushes.Black;
                BtnTableau.FontWeight = FontWeights.Bold;
                BtnGraphique.Background = Brushes.Transparent;
                BtnGraphique.Foreground = (Brush)App.Current.Resources["ZenButtonTextBrush"];
                BtnGraphique.FontWeight = FontWeights.Normal;
            }
        }
    }
}
