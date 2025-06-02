using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Concentrade
{
    public partial class RunningAppsPopup : Window
    {
        public ObservableCollection<RunningApp> RunningApps { get; set; }
        private readonly AppBlocker _appBlocker;

        public RunningAppsPopup(AppBlocker appBlocker)
        {
            InitializeComponent();
            _appBlocker = appBlocker;
            RunningApps = new ObservableCollection<RunningApp>();
            AppsList.ItemsSource = RunningApps;
            LoadRunningApps();
        }

        private void LoadRunningApps()
        {
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
                            IsSelected = true // Par défaut, on sélectionne toutes les applications
                        });
                    }
                }
                catch { }
            }

            // Si aucune application distrayante n'est trouvée, fermer directement la fenêtre
            if (RunningApps.Count == 0)
            {
                Close();
            }
        }

        private void CloseSelected_Click(object sender, RoutedEventArgs e)
        {
            foreach (var app in RunningApps)
            {
                if (app.IsSelected)
                {
                    try
                    {
                        app.Process?.Kill();
                    }
                    catch { }
                }
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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