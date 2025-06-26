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

        public bool TemporarilyAllowed { get; private set; }
        public TimeSpan AllowedDuration { get; private set; }

        private readonly Process? _processToBlock;
        private readonly DispatcherTimer _focusTimer;

        // Constructeur par défaut
        public BlocagePopup()
        {
            InitializeComponent();
        }

        // Initialise le popup de blocage avec le nom de l'application et le processus
        public BlocagePopup(string appName, Process processToBlock)
        {
            InitializeComponent();
            this.Topmost = true;
            _processToBlock = processToBlock;

            _focusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _focusTimer.Tick += FocusTimer_Tick;
            this.Loaded += (s, e) => _focusTimer.Start();
            this.Closed += (s, e) => _focusTimer.Stop();

            MessageText.Text = $"{appName} est bloqué.\nAutoriser temporairement ou fermer ?";

            TimeSelection.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
            TimeSelection.BorderBrush = new SolidColorBrush(Colors.DimGray);
            TimeSelection.Foreground = Brushes.White;

            var itemStyle = new Style(typeof(ComboBoxItem));

            itemStyle.Setters.Add(new Setter(ComboBoxItem.BackgroundProperty, new SolidColorBrush(Color.FromRgb(45, 45, 48))));
            itemStyle.Setters.Add(new Setter(ComboBoxItem.ForegroundProperty, Brushes.White));

            var mouseOverTrigger = new Trigger { Property = IsMouseOverProperty, Value = true };
            mouseOverTrigger.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Colors.DimGray)));
            itemStyle.Triggers.Add(mouseOverTrigger);

            TimeSelection.ItemContainerStyle = itemStyle;

            TimeSelection.Items.Clear();
            TimeSelection.ItemsSource = Enumerable.Range(1, 10).Select(i => $"{i} minute{(i > 1 ? "s" : "")}");
            TimeSelection.SelectedIndex = 4;

            AlwaysAllowCheckbox.Visibility = Visibility.Collapsed;
            TimeSelectionGrid.Visibility = Visibility.Visible;
            ContinueButton.Content = "Autoriser";
        }

        // Gère le clic sur le bouton autoriser
        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            TemporarilyAllowed = true;
            AllowedDuration = GetSelectedTimeSpan();
            DialogResult = true;
            Close();
        }

        // Gère le clic sur le bouton fermer
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            TemporarilyAllowed = false;
            DialogResult = false;
            Close();
        }

        // Récupère la durée sélectionnée dans la combobox
        private TimeSpan GetSelectedTimeSpan()
        {
            if (TimeSelection?.SelectedItem is string selectedItem)
            {
                if (int.TryParse(selectedItem.Split(' ')[0], out int minutes))
                {
                    return TimeSpan.FromMinutes(minutes);
                }
            }
            return TimeSpan.FromMinutes(5);
        }

        // Garde le focus sur le popup et minimise l'application bloquée
        private void FocusTimer_Tick(object? sender, EventArgs e)
        {
            if (_processToBlock == null || _processToBlock.HasExited)
            {
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