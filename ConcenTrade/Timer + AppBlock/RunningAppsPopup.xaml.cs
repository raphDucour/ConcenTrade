using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Concentrade
{
    public partial class RunningAppsPopup : Window
    {
        public ObservableCollection<RunningApp> RunningApps { get; }
        private readonly AppBlocker? _appBlocker;
        public bool ContinueWithoutClosing { get; private set; }

        // Initialise le popup des applications en cours d'exécution
        public RunningAppsPopup(AppBlocker appBlocker)
        {
            InitializeComponent();
            _appBlocker = appBlocker;
            RunningApps = new ObservableCollection<RunningApp>();
            ContinueWithoutClosing = false;
            AppsList.ItemsSource = RunningApps;
            LoadRunningApps();
        }

        // Charge la liste des applications distrayantes en cours d'exécution
        private void LoadRunningApps()
        {
            if (_appBlocker == null) return;

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle) && _appBlocker.IsDistractingApp(process.ProcessName))
                    {
                        RunningApps.Add(new RunningApp
                        {
                            Name = process.ProcessName,
                            Description = process.MainWindowTitle,
                            Process = process,
                            IsSelected = true
                        });
                    }
                }
                catch { }
            }

            if (RunningApps.Count == 0)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    DialogResult = true;
                    Close();
                }));
            }
        }

        // Ferme les applications sélectionnées
        private void CloseSelected_Click(object sender, RoutedEventArgs e)
        {
            ContinueWithoutClosing = false;
            DialogResult = true;
            Close();
        }

        // Continue sans fermer les applications
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ContinueWithoutClosing = true;
            DialogResult = true;
            Close();
        }
    }

    public class RunningApp
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required Process Process { get; set; }
        public bool IsSelected { get; set; }
    }
} 