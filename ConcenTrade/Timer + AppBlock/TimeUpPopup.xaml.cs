using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace Concentrade
{
    public partial class TimeUpPopup : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MINIMIZE = 6;

        private readonly Process? _processToBlock;
        private readonly System.Windows.Threading.DispatcherTimer _focusTimer;

        public enum TimeUpAction { Close, Extend }
        public TimeUpAction Action { get; private set; }
        public TimeSpan ExtensionDuration { get; private set; }

        public TimeUpPopup(string appName, Process processToBlock)
        {
            InitializeComponent();
            this.Topmost = true;

            _processToBlock = processToBlock;

            _focusTimer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _focusTimer.Tick += FocusTimer_Tick;
            this.Loaded += (s, e) => _focusTimer.Start();
            this.Closed += (s, e) => _focusTimer.Stop();

            DurationComboBox.ItemsSource = Enumerable.Range(1, 5).Select(i => $"{i} minute{(i > 1 ? "s" : "")}");
            DurationComboBox.SelectedIndex = 1;
        }

        private void FocusTimer_Tick(object? sender, EventArgs e)
        {
            if (_processToBlock == null || _processToBlock.HasExited) { _focusTimer.Stop(); this.Close(); return; }
            try
            {
                if (_processToBlock.MainWindowHandle != IntPtr.Zero) ShowWindow(_processToBlock.MainWindowHandle, SW_MINIMIZE);
                this.Activate();
            }
            catch { _focusTimer.Stop(); }
        }

        private void ExtendButton_Click(object sender, RoutedEventArgs e)
        {
            Action = TimeUpAction.Extend;
            string selected = DurationComboBox.SelectedItem as string ?? "2 minutes";
            int minutes = int.Parse(selected.Split(' ')[0]);
            ExtensionDuration = TimeSpan.FromMinutes(minutes);
            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Action = TimeUpAction.Close;
            DialogResult = true;
            Close();
        }
    }
}