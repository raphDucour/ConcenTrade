using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;

namespace Concentrade
{
    public partial class BlockedAppsSettings : Window
    {
        // Dictionnaire qui associe un lanceur à ses processus enfants/liés connus.
        // C'est une copie de la logique de AppBlocker pour vérifier les conflits.
        private readonly Dictionary<string, List<string>> _launcherChildren = new()
            {
                { "steam", new List<string> { "steamwebhelper", "steamservice", "gameoverlayui" } },
                { "epicgameslauncher", new List<string> { "epicgameslauncherhelper", "epicwebhelper" } },
                { "battle.net", new List<string> { "blizzardbrowser", "agent" } },
                { "ea", new List<string> { "eadestop", "eabackgroundservice", "ealauncher" } },
                { "ubisoftconnect", new List<string> { "upc", "uplay" } },
                { "riot", new List<string> { "riotclientservices", "valorant-win64-shipping" } }
            };

        // On ajoute le même dictionnaire d'alias que dans AppBlocker
        private readonly Dictionary<string, string> _appAliases = new()
        {
            { "lol", "league of legends" },
            { "battlenet", "battle.net" },
            { "chrome", "google chrome" },
            { "msedge", "microsoft edge" },
            { "firefox", "mozilla firefox" }
        };

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

        public BlockedAppsSettings(string[] currentBlockedApps)
        {
            InitializeComponent();
            BlockedApps = currentBlockedApps;
            LoadBlockedApps();
        }

        private void LoadBlockedApps()
        {
            // Liste des applications suggérées par défaut.
            var defaultApps = new[]
            {
        "Discord", "Slack", "Telegram Desktop", "WhatsApp", "Twitter",
        "Spotify", "Netflix", "Prime Video", "DisneyPlus", "Twitch",
        "Steam", "EpicGamesLauncher", "EA", "UbisoftConnect", "Battle.net", "Riot",
        "Google Chrome", "Mozilla Firefox", "Opera", "OperaGX", "Microsoft Edge"
    };

            // On utilise un HashSet pour une recherche rapide et insensible à la casse
            // de ce que l'utilisateur a VRAIMENT sauvegardé comme bloqué.
            // La propriété 'BlockedApps' contient cette liste, passée au constructeur.
            var userBlockedAppsSet = new HashSet<string>(BlockedApps, StringComparer.OrdinalIgnoreCase);

            // Ce second HashSet nous aide à ne pas ajouter deux fois la même application à l'interface.
            var allAppsInUi = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // --- ÉTAPE 1: Parcourir les applications par défaut ---
            // Une application ne sera cochée que si elle est dans la liste sauvegardée de l'utilisateur.
            foreach (var appName in defaultApps)
            {
                _apps.Add(new AppItem(appName, userBlockedAppsSet.Contains(appName)));
                allAppsInUi.Add(appName);
            }

            // --- ÉTAPE 2: Ajouter les applications personnalisées ---
            // Si l'utilisateur a ajouté une application qui n'est pas dans la liste par défaut,
            // on s'assure qu'elle soit bien présente et cochée.
            foreach (var userApp in userBlockedAppsSet)
            {
                if (!allAppsInUi.Contains(userApp))
                {
                    _apps.Add(new AppItem(userApp, true));
                }
            }

            // On lie la liste finale à l'interface utilisateur.
            SuggestedAppsList.ItemsSource = _apps;
        }

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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtenir la liste de toutes les applications sélectionnées par l'utilisateur
            var selectedApps = _apps.Where(a => a.IsSelected)
                                  .Select(a => a.Name.ToLower())
                                  .ToList();

            // 2. Vérifier les conflits
            string conflictMessage = "";
            var launchers = _launcherChildren.Keys;

            // On parcourt les lanceurs sélectionnés par l'utilisateur
            foreach (var selectedLauncher in selectedApps.Intersect(launchers))
            {
                // On récupère les processus enfants de ce lanceur
                var children = _launcherChildren[selectedLauncher];

                // On cherche si un de ces processus enfants est AUSSI dans la liste des sélections
                var conflictingChildren = selectedApps.Intersect(children).ToList();

                if (conflictingChildren.Any())
                {
                    conflictMessage += $"Le lanceur '{selectedLauncher}' est redondant avec : {string.Join(", ", conflictingChildren)}.\n";
                }
            }

            // 3. Afficher un message d'avertissement s'il y a des conflits
            if (!string.IsNullOrEmpty(conflictMessage))
            {
                conflictMessage += "\nIl est recommandé de bloquer uniquement le lanceur principal. Voulez-vous sauvegarder quand même ?";

                MessageBoxResult result = MessageBox.Show(this, conflictMessage, "Avertissement de redondance", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                // Si l'utilisateur clique sur "Non", on arrête le processus de sauvegarde
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            // 4. Si tout va bien (ou si l'utilisateur a cliqué "Oui"), on sauvegarde et on ferme.
            BlockedApps = selectedApps.ToArray();
            DialogResult = true;
            Close();
        }
    }
} 