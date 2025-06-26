using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Concentrade
{
    public partial class DistractingAppsConfirmation : Window
    {
        public ObservableCollection<RunningApp> RunningApps { get; set; }
        public bool ContinueWithoutClosing { get; private set; }

        // Initialise la fenêtre de confirmation des applications distrayantes
        public DistractingAppsConfirmation(AppBlocker appBlocker)
        {
            InitializeComponent();
            RunningApps = new ObservableCollection<RunningApp>();
            AppsList.ItemsSource = RunningApps;
            LoadRunningApps(appBlocker);
        }

        // Charge la liste des applications distrayantes en cours d'exécution
        private void LoadRunningApps(AppBlocker appBlocker)
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle) && appBlocker.IsDistractingApp(process.ProcessName))
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
                DialogResult = true;
                Close();
            }
        }

        // Continue sans fermer les applications sélectionnées
        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            ContinueWithoutClosing = true;
            DialogResult = true;
            Close();
        }

        // Ferme les applications sélectionnées
        private void CloseSelected_Click(object sender, RoutedEventArgs e)
        {
            ContinueWithoutClosing = false;
            DialogResult = true;
            Close();
        }
    }
} 