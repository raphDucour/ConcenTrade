using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace Concentrade
{
    public partial class FocusModePopup : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MINIMIZE = 6;

        private readonly Process _processToBlock;
        private readonly DispatcherTimer _focusTimer;

        // Initialise le popup de mode focus avec le nom de l'application et le processus
        public FocusModePopup(string appName, Process processToBlock)
        {
            InitializeComponent();
            MessageText.Text = $"L'application '{appName}' est une distraction. En Mode Focus, elle doit être fermée.";
            _processToBlock = processToBlock;

            _focusTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _focusTimer.Tick += FocusTimer_Tick;
            this.Loaded += (s, e) => _focusTimer.Start();
            this.Closed += (s, e) => _focusTimer.Stop();
        }

        // Ferme l'application distrayante
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_processToBlock.HasExited)
                {
                    _processToBlock.Kill(true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Impossible de fermer le processus : {ex.Message}");
            }
            this.Close();
        }

        // Garde le focus sur le popup et minimise l'application distrayante
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
                this.Activate();
                if (_processToBlock.MainWindowHandle != IntPtr.Zero)
                {
                    ShowWindow(_processToBlock.MainWindowHandle, SW_MINIMIZE);
                }
            }
            catch { _focusTimer.Stop(); }
        }
    }
}