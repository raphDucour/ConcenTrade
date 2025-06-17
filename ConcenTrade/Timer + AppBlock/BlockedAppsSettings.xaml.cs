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
            // Suggestions par défaut plus orientées PC
            var defaultApps = new[]
            {
        // Communication & Réseaux Sociaux
        "Discord", "Slack", "Telegram Desktop", "WhatsApp", "Twitter",
        // Divertissement & Média
        "Spotify", "Netflix", "Prime Video", "DisneyPlus", "Twitch",
        // Lanceurs de jeux
        "Steam", "EpicGamesLauncher", "EA", "UbisoftConnect", "Battle.net",
        // Navigateurs Web
        "Chrome", "Firefox", "Opera", "OperaGX", "msedge"
    };

            // Laisser les lanceurs de jeux décochés par défaut
            var uncheckedByDefault = new[] {
        "steam", "epicgameslauncher", "ea", "ubisoftconnect", "battle.net"
    };

            // Ajouter les applications qui sont déjà bloquées par l'utilisateur (elles seront cochées)
            foreach (var app in BlockedApps)
            {
                if (!_apps.Any(a => a.Name.Equals(app, StringComparison.OrdinalIgnoreCase)))
                {
                    _apps.Add(new AppItem(app, true));
                }
            }

            // Ajouter les suggestions de la liste par défaut si elles n'ont pas déjà été ajoutées
            foreach (var app in defaultApps)
            {
                if (!_apps.Any(a => a.Name.Equals(app, StringComparison.OrdinalIgnoreCase)))
                {
                    // On coche la case sauf si l'app est dans notre liste "uncheckedByDefault"
                    bool isSelected = !uncheckedByDefault.Contains(app.ToLower());
                    _apps.Add(new AppItem(app, isSelected));
                }
            }

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