using System;
using System.Collections.ObjectModel;
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
            // Suggestions par défaut
            var defaultApps = new[]
            {
                "Discord",
                "Spotify",
                "TikTok",
                "Chrome",
                "Microsoft Edge",
                "Firefox",
                "Opera",
                "Steam",
                "Epic Games Launcher"
            };

            // Ajouter les applications actuellement bloquées
            foreach (var app in BlockedApps)
            {
                _apps.Add(new AppItem(app));
            }

            // Ajouter les suggestions qui ne sont pas déjà dans la liste
            foreach (var app in defaultApps)
            {
                if (!_apps.Any(a => a.Name.Equals(app, StringComparison.OrdinalIgnoreCase)))
                {
                    _apps.Add(new AppItem(app, false));
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
            // Retourner uniquement les applications sélectionnées
            BlockedApps = _apps.Where(a => a.IsSelected)
                              .Select(a => a.Name.ToLower())
                              .ToArray();
            DialogResult = true;
            Close();
        }
    }
} 