using System;
using System.Windows;
using System.Windows.Controls;

namespace Concentrade
{
    /// <summary>
    /// Logique d'interaction pour BlocagePopup.xaml
    /// </summary>
    public partial class BlocagePopup : Window
    {
        public bool ContinueAnyway { get; private set; }
        public bool TemporarilyAllowed { get; private set; }
        public TimeSpan AllowedDuration { get; private set; }

        // Constructeur par défaut requis par XAML
        public BlocagePopup()
        {
            InitializeComponent();
            SetupWindow();
        }

        // Constructeur avec le nom de l'application
        public BlocagePopup(string appName) : this()
        {
            this.Topmost = true; // <--- AJOUTEZ CETTE LIGNE
            if (MessageText != null)
            {
                MessageText.Text = appName != null
                    ? $"{appName} est bloqué pendant ta session.\nSouhaites-tu vraiment l'ouvrir ?"
                    : "Cette application est bloquée pendant ta session.\nSouhaites-tu vraiment l'ouvrir ?";
            }
        }

        private void SetupWindow()
        {
            ContinueAnyway = false;
            TemporarilyAllowed = false;
            AllowedDuration = TimeSpan.Zero;

            // Gérer la visibilité de la sélection du temps
            if (AlwaysAllowCheckbox != null && TimeSelectionGrid != null)
            {
                AlwaysAllowCheckbox.Checked += (s, e) =>
                {
                    if (TimeSelectionGrid != null)
                    {
                        TimeSelectionGrid.Visibility = Visibility.Visible;
                    }
                };

                AlwaysAllowCheckbox.Unchecked += (s, e) =>
                {
                    if (TimeSelectionGrid != null)
                    {
                        TimeSelectionGrid.Visibility = Visibility.Collapsed;
                    }
                };
            }

            // Sélectionner la première option par défaut
            if (TimeSelection != null)
            {
                TimeSelection.SelectedIndex = 0;
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            ContinueAnyway = true;
            TemporarilyAllowed = AlwaysAllowCheckbox?.IsChecked ?? false;

            if (TemporarilyAllowed)
            {
                AllowedDuration = GetSelectedTimeSpan();
            }

            DialogResult = true;
            Close();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            ContinueAnyway = false;
            TemporarilyAllowed = false;
            DialogResult = false;
            Close();
        }

        private TimeSpan GetSelectedTimeSpan()
        {
            if (TimeSelection?.SelectedItem is ComboBoxItem selectedItem)
            {
                string? content = selectedItem.Content?.ToString();
                if (!string.IsNullOrEmpty(content))
                {
                    if (content.Contains("heure", StringComparison.OrdinalIgnoreCase))
                    {
                        return TimeSpan.FromHours(1);
                    }
                    else
                    {
                        string[] parts = content.Split(' ');
                        if (parts.Length > 0 && int.TryParse(parts[0], out int result))
                        {
                            return TimeSpan.FromMinutes(result);
                        }
                    }
                }
            }
            return TimeSpan.FromMinutes(5); // Valeur par défaut
        }
    }
}