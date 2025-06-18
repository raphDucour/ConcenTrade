using System;
using System.Collections.ObjectModel;
using System.Collections.Generic; // Assurez-vous que cette ligne est présente
using System.Windows;
using System.Linq;

namespace Concentrade
{
    public partial class BlockedAppsSettings : Window
    {
        // (La classe interne AppItem reste la même)
        public class AppItem
        {
            public string Name { get; set; }
            public bool IsSelected { get; set; }

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

        // Le constructeur accepte maintenant aussi la liste des ignorées
        public BlockedAppsSettings(string[] currentBlockedApps, List<string> currentIgnoredApps)
        {
            InitializeComponent();
            BlockedApps = currentBlockedApps;
            _initialIgnoredApps = currentIgnoredApps; // On stocke la liste initiale
            IgnoredApps = new List<string>();       // On initialise la liste de retour
            LoadBlockedApps();
        }

        private void LoadBlockedApps()
        {
            var userBlockedAppsSet = new HashSet<string>(BlockedApps, StringComparer.OrdinalIgnoreCase);
            var userIgnoredAppsSet = new HashSet<string>(_initialIgnoredApps, StringComparer.OrdinalIgnoreCase);
            var allAppsInUi = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // ÉTAPE 1: Afficher les applications par défaut SAUF si elles sont ignorées
            foreach (var appName in _defaultApps)
            {
                if (!userIgnoredAppsSet.Contains(appName))
                {
                    _apps.Add(new AppItem(appName, userBlockedAppsSet.Contains(appName)));
                    allAppsInUi.Add(appName);
                }
            }

            // ÉTAPE 2: Ajouter les applications personnalisées sauvegardées par l'utilisateur
            foreach (var userApp in userBlockedAppsSet)
            {
                if (!allAppsInUi.Contains(userApp))
                {
                    _apps.Add(new AppItem(userApp, true));
                }
            }

            SuggestedAppsList.ItemsSource = _apps;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // La logique de vérification des conflits (redondance) reste la même...

            // On sauvegarde les applications qui sont cochées
            BlockedApps = _apps.Where(a => a.IsSelected)
                                 .Select(a => a.Name)
                                 .ToArray();

            // NOUVELLE LOGIQUE : On détermine la nouvelle liste d'applications ignorées
            var currentAppsInUi = new HashSet<string>(_apps.Select(a => a.Name), StringComparer.OrdinalIgnoreCase);

            // Une application par défaut est "ignorée" si elle n'est plus visible dans la liste
            IgnoredApps = _defaultApps
                .Where(defaultApp => !currentAppsInUi.Contains(defaultApp))
                .ToList();

            DialogResult = true;
            Close();
        }

        // Les autres méthodes (AddApp_Click, RemoveApp_Click, Cancel_Click) restent inchangées...
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

        private void RemoveApp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is AppItem app)
            {
                _apps.Remove(app);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}