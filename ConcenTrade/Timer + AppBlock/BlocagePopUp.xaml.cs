using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Concentrade
{
    public partial class BlocagePopup : Window
    {
        // Import d'une fonction Windows pour minimiser une fenêtre
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MINIMIZE = 6;

        public bool ContinueAnyway { get; private set; }
        public bool TemporarilyAllowed { get; private set; }
        public TimeSpan AllowedDuration { get; private set; }

        // Variable pour garder en mémoire le processus du jeu à bloquer
        private readonly Process? _processToBlock;
        // Le minuteur qui va se battre pour garder le focus
        private readonly DispatcherTimer _focusTimer;

        // Constructeur par défaut (nécessaire pour XAML)
        public BlocagePopup()
        {
            InitializeComponent();
            SetupWindow();
        }

        // Nouveau constructeur que nous utilisons depuis AppBlocker
        public BlocagePopup(string appName, Process processToBlock)
        {
            InitializeComponent();
            SetupWindow();

            this.Topmost = true; // S'assure que la fenêtre reste au-dessus
            _processToBlock = processToBlock; // On stocke le processus du jeu

            // Configuration du minuteur qui s'exécutera 4 fois par seconde
            _focusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            _focusTimer.Tick += FocusTimer_Tick;

            MessageText.Text = $"{appName} est bloqué pendant ta session.\nSouhaites-tu vraiment l'ouvrir ?";

            // On démarre et arrête le minuteur en même temps que la fenêtre
            this.Loaded += (s, e) => _focusTimer.Start();
            this.Closed += (s, e) => _focusTimer.Stop();
        }

        // C'est le cœur de la solution : cette méthode est appelée 4 fois par seconde
        private void FocusTimer_Tick(object? sender, EventArgs e)
        {
            if (_processToBlock == null || _processToBlock.HasExited)
            {
                _focusTimer.Stop(); // Arrête le minuteur si le jeu est fermé
                this.Close(); // Ferme le popup
                return;
            }

            try
            {
                // On force la réduction de la fenêtre du jeu
                if (_processToBlock.MainWindowHandle != IntPtr.Zero)
                {
                    ShowWindow(_processToBlock.MainWindowHandle, SW_MINIMIZE);
                }
                // Et on s'assure que notre popup est bien la fenêtre active
                this.Activate();
            }
            catch
            {
                _focusTimer.Stop(); // Sécurité en cas d'erreur
            }
        }

        // --- Les méthodes ci-dessous restent inchangées ---

        private void SetupWindow()
        {
            ContinueAnyway = false;
            TemporarilyAllowed = false;
            AllowedDuration = TimeSpan.Zero;

            if (AlwaysAllowCheckbox != null && TimeSelectionGrid != null)
            {
                AlwaysAllowCheckbox.Checked += (s, e) => TimeSelectionGrid.Visibility = Visibility.Visible;
                AlwaysAllowCheckbox.Unchecked += (s, e) => TimeSelectionGrid.Visibility = Visibility.Collapsed;
            }
            if (TimeSelection != null) TimeSelection.SelectedIndex = 0;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            ContinueAnyway = true;
            TemporarilyAllowed = AlwaysAllowCheckbox?.IsChecked ?? false;
            if (TemporarilyAllowed) AllowedDuration = GetSelectedTimeSpan();
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
            if (TimeSelection?.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content?.ToString() is string content)
            {
                if (content.Contains("heure", StringComparison.OrdinalIgnoreCase)) return TimeSpan.FromHours(1);
                if (int.TryParse(content.Split(' ')[0], out int minutes)) return TimeSpan.FromMinutes(minutes);
            }
            return TimeSpan.FromMinutes(5);
        }
    }
}