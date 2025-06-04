using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Concentrade
{
    public partial class RunningAppsPopup : Window
    {
        public ObservableCollection<RunningApp> RunningApps { get; set; }
        private readonly AppBlocker _appBlocker;
        public bool ContinueWithoutClosing { get; private set; }

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
                // On utilise Dispatcher.BeginInvoke pour s'assurer que la fenêtre est complètement initialisée
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    DialogResult = true;
                    Close();
                }));
            }
        }

        private void CloseSelected_Click(object sender, RoutedEventArgs e)
        {
            ContinueWithoutClosing = false;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ContinueWithoutClosing = true;
            DialogResult = true;
            Close();
        }
    }

    public class RunningApp
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Process Process { get; set; }
        public bool IsSelected { get; set; }
    }
} 