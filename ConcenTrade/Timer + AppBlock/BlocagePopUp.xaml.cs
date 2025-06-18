using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Concentrade
{
    public partial class BlocagePopup : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MINIMIZE = 6;

        // La seule information de sortie qui nous intéresse maintenant.
        public bool TemporarilyAllowed { get; private set; }
        public TimeSpan AllowedDuration { get; private set; }

        private readonly Process? _processToBlock;
        private readonly DispatcherTimer _focusTimer;

        // Constructeur par défaut
        public BlocagePopup()
        {
            InitializeComponent();
        }

        // Constructeur principal, qui configure notre nouvelle logique
        // Dans le fichier Concentrade/Timer + AppBlock/BlocagePopUp.xaml.cs

        public BlocagePopup(string appName, Process processToBlock)
        {
            InitializeComponent();
            this.Topmost = true;
            _processToBlock = processToBlock;

            // Logique pour garder le focus (de la réponse précédente)
            _focusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _focusTimer.Tick += FocusTimer_Tick;
            this.Loaded += (s, e) => _focusTimer.Start();
            this.Closed += (s, e) => _focusTimer.Stop();

            MessageText.Text = $"{appName} est bloqué.\nAutoriser temporairement ou fermer ?";

            // --- NOUVEAU BLOC POUR LE DESIGN ---

            // 1. On stylise la ComboBox elle-même pour qu'elle soit sombre
            TimeSelection.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48)); // Un gris très sombre
            TimeSelection.BorderBrush = new SolidColorBrush(Colors.DimGray);
            TimeSelection.Foreground = Brushes.White;

            // 2. On crée un style pour les éléments de la liste déroulante
            var itemStyle = new Style(typeof(ComboBoxItem));

            // Couleur de fond et de texte pour chaque option
            itemStyle.Setters.Add(new Setter(ComboBoxItem.BackgroundProperty, new SolidColorBrush(Color.FromRgb(45, 45, 48))));
            itemStyle.Setters.Add(new Setter(ComboBoxItem.ForegroundProperty, Brushes.White));

            // On crée un "déclencheur" pour changer la couleur quand la souris passe sur une option
            var mouseOverTrigger = new Trigger { Property = IsMouseOverProperty, Value = true };
            mouseOverTrigger.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.DimGray)));
            itemStyle.Triggers.Add(mouseOverTrigger);

            // On applique ce style aux éléments de notre ComboBox
            TimeSelection.ItemContainerStyle = itemStyle;

            // --- FIN DU BLOC POUR LE DESIGN ---

            // Logique pour remplir la ComboBox (de la réponse précédente)
            TimeSelection.Items.Clear();
            TimeSelection.ItemsSource = Enumerable.Range(1, 10).Select(i => $"{i} minute{(i > 1 ? "s" : "")}");
            TimeSelection.SelectedIndex = 4; // Défaut à 5 minutes

            // On cache les anciens contrôles
            AlwaysAllowCheckbox.Visibility = Visibility.Collapsed;
            TimeSelectionGrid.Visibility = Visibility.Visible;
            ContinueButton.Content = "Autoriser";
        }
        // Le clic sur "Autoriser" (anciennement "Continuer")
        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            TemporarilyAllowed = true;
            AllowedDuration = GetSelectedTimeSpan();
            DialogResult = true;
            Close();
        }

        // Le clic sur "Fermer" ne change pas
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            TemporarilyAllowed = false;
            DialogResult = false;
            Close();
        }

        private TimeSpan GetSelectedTimeSpan()
        {
            if (TimeSelection?.SelectedItem is string selectedItem)
            {
                if (int.TryParse(selectedItem.Split(' ')[0], out int minutes))
                {
                    return TimeSpan.FromMinutes(minutes);
                }
            }
            return TimeSpan.FromMinutes(5); // Valeur par défaut
        }

        private void FocusTimer_Tick(object? sender, EventArgs e)
        {
            if (_processToBlock == null || _processToBlock.HasExited)
            {
                _focusTimer.Stop();
                this.Close();
                return;
            }
            try
            {
                if (_processToBlock.MainWindowHandle != IntPtr.Zero) ShowWindow(_processToBlock.MainWindowHandle, SW_MINIMIZE);
                this.Activate();
            }
            catch { _focusTimer.Stop(); }
        }

        // Méthode non utilisée mais requise par le XAML existant
        private void SetupWindow() { }
    }
}