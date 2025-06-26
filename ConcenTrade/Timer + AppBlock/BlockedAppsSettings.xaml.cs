using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace Concentrade
{
    public partial class BlockedAppsSettings : Window
    {
        public class AppItem
        {
            public string Name { get; set; }
            public bool IsSelected { get; set; }

            // Initialise un élément d'application avec nom et état de sélection
            public AppItem(string name, bool isSelected = true)
            {
                Name = name;
                IsSelected = isSelected;
            }
        }

        private ObservableCollection<AppItem> _apps = new();
        public string[] BlockedApps { get; private set; }

        // NOUVELLE PROPRIÉTÉ pour renvoyer la liste des applications ignorées
        public List<string> IgnoredApps { get; private set; }

        // La liste initiale des applications ignorées, reçue au démarrage
        private readonly List<string> _initialIgnoredApps;

        private readonly string[] _defaultApps = new[]
        {
            "Discord", "Slack", "Telegram Desktop", "WhatsApp", "Twitter",
            "Spotify", "Netflix", "Prime Video", "DisneyPlus", "Twitch",
            "Steam", "EpicGamesLauncher", "EA", "UbisoftConnect", "Battle.net", "Riot",
            "Google Chrome", "Mozilla Firefox", "Opera", "OperaGX", "Microsoft Edge"
        };

        // Initialise les paramètres des applications bloquées
        public BlockedAppsSettings(string[] currentBlockedApps, List<string> currentIgnoredApps)
        {
            InitializeComponent();
            BlockedApps = currentBlockedApps;
            _initialIgnoredApps = currentIgnoredApps;
            IgnoredApps = new List<string>();
            LoadBlockedApps();
        }

        // Charge et affiche la liste des applications bloquées
        private void LoadBlockedApps()
        {
            var userBlockedAppsSet = new HashSet<string>(BlockedApps, StringComparer.OrdinalIgnoreCase);
            var userIgnoredAppsSet = new HashSet<string>(_initialIgnoredApps, StringComparer.OrdinalIgnoreCase);
            var allAppsInUi = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var appName in _defaultApps)
            {
                if (!userIgnoredAppsSet.Contains(appName))
                {
                    _apps.Add(new AppItem(appName, userBlockedAppsSet.Contains(appName)));
                    allAppsInUi.Add(appName);
                }
            }

            foreach (var userApp in userBlockedAppsSet)
            {
                if (!allAppsInUi.Contains(userApp))
                {
                    _apps.Add(new AppItem(userApp, true));
                }
            }

            SuggestedAppsList.ItemsSource = _apps;
        }

        // Sauvegarde les applications sélectionnées et détermine les ignorées
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            BlockedApps = _apps.Where(a => a.IsSelected)
                                 .Select(a => a.Name)
                                 .ToArray();

            var currentAppsInUi = new HashSet<string>(_apps.Select(a => a.Name), StringComparer.OrdinalIgnoreCase);

            IgnoredApps = _defaultApps
                .Where(defaultApp => !currentAppsInUi.Contains(defaultApp))
                .ToList();

            DialogResult = true;
            Close();
        }

        // Ajoute une nouvelle application à la liste
        private void AddApp_Click(object sender, RoutedEventArgs e)
        {
            var newApp = NewAppTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newApp)) return;

            if (!_apps.Any(a => a.Name.Equals(newApp, StringComparison.OrdinalIgnoreCase)))
            {
                _apps.Add(new AppItem(newApp));
                NewAppTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Cette application est déjà dans la liste.", "Application existante",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Supprime une application de la liste
        private void RemoveApp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is AppItem app)
            {
                _apps.Remove(app);
            }
        }

        // Annule les modifications et ferme la fenêtre
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}