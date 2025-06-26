using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using Concentrade.Properties;

namespace Concentrade
{
    public class AppBlocker
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MINIMIZE = 6;
        private bool _isFocusModeSessionActive = false;

        private ManagementEventWatcher? _watcher;
        private readonly object _promptLock = new object();
        private bool _isActive = false;

        private List<string> _blockedApps = LoadBlockedApps();

        private readonly Dictionary<string, string> _displayNameCache = new();

        private DateTime _lastGlobalPromptTime = DateTime.MinValue;
        private readonly TimeSpan _popupCooldown = TimeSpan.FromSeconds(15);

        private readonly Dictionary<string, DateTime> _temporaryAllowances = new();

        public event EventHandler<TemporaryAllowanceEventArgs>? OnTemporaryAllowance;

        // Autorise temporairement une application pour une durée donnée
        public void AllowTemporarily(string processName, TimeSpan duration)
        {
            var mainProcessName = GetMainProcessName(processName);
            _temporaryAllowances[mainProcessName] = DateTime.Now.Add(duration);
            OnTemporaryAllowance?.Invoke(this, new TemporaryAllowanceEventArgs(mainProcessName, duration));
        }

        // Initialise le bloqueur d'applications
        public AppBlocker()
        {
        }

        // Met à jour la liste des applications bloquées
        public void UpdateBlockedApps(IEnumerable<string> newBlockedApps)
        {
            _blockedApps = new List<string>(newBlockedApps.Select(app => NormalizeAppName(app)));
            _displayNameCache.Clear();
        }

        // Retourne la liste des applications bloquées
        public List<string> GetBlockedApps()
        {
            return _blockedApps;
        }

        // Normalise le nom d'une application en utilisant les alias
        private string NormalizeAppName(string appName)
        {
            appName = appName.ToLower().Trim();

            if (_appAliases.TryGetValue(appName, out string? aliasedName))
            {
                return aliasedName;
            }

            return appName;
        }

        // Démarre la surveillance des processus
        public void Start()
        {
            try
            {
                var query = new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace");
                _watcher = new ManagementEventWatcher(query);
                _watcher.EventArrived += OnProcessStarted;
                _watcher.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur au démarrage du AppBlocker : {ex.Message}");
                MessageBox.Show("Impossible de démarrer la surveillance des applications. Veuillez vérifier vos droits d'administrateur.", "Erreur AppBlocker");
            }
        }

        // Gère l'événement de démarrage d'un nouveau processus
        private void OnProcessStarted(object sender, EventArrivedEventArgs e)
        {
            if (!_isActive) return;

            string? processName = e.NewEvent.Properties["ProcessName"].Value?.ToString();
            if (string.IsNullOrEmpty(processName)) return;

            Task.Run(async () =>
            {
                try
                {
                    string originalProcessName = Path.GetFileNameWithoutExtension(processName).ToLower();
                    int processId = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);

                    if (IsDistractingApp(originalProcessName))
                    {
                        Process? process = null;
                        try { process = Process.GetProcessById(processId); }
                        catch { return; }

                        var mainProcessName = GetMainProcessName(originalProcessName);

                        lock (_promptLock)
                        {
                            if ((DateTime.Now - _lastGlobalPromptTime) < _popupCooldown)
                            {
                                return;
                            }
                        }

                        if (_temporaryAllowances.TryGetValue(mainProcessName, out DateTime expirationTime))
                        {
                            if (DateTime.Now < expirationTime) return;
                            _temporaryAllowances.Remove(mainProcessName);
                        }

                        bool isGame = mainProcessName.Contains("game") || mainProcessName.Contains("shipping") || _relatedProcesses.ContainsKey(mainProcessName);
                        bool windowReady = isGame || await WaitForMainWindow(process, TimeSpan.FromSeconds(5));
                        if (!windowReady && !isGame) return;

                        if (process.MainWindowHandle != IntPtr.Zero)
                        {
                            Application.Current.Dispatcher.Invoke(() => ShowWindow(process.MainWindowHandle, SW_MINIMIZE));
                            await Task.Delay(100);
                        }

                        string displayName = await GetDisplayName(process, mainProcessName);

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            lock (_promptLock)
                            {
                                if ((DateTime.Now - _lastGlobalPromptTime) < _popupCooldown) return;
                                _lastGlobalPromptTime = DateTime.Now;
                            }

                            if (process.MainWindowHandle != IntPtr.Zero)
                            {
                                ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
                            }

                            if (_isFocusModeSessionActive)
                            {
                                var focusPopup = new FocusModePopup(displayName, process);
                                focusPopup.ShowDialog();
                            }
                            else
                            {
                                var popup = new BlocagePopup(displayName, process);
                                popup.Activate();
                                bool? dialogResult = popup.ShowDialog();

                                if (dialogResult == true && popup.TemporarilyAllowed)
                                {
                                    AllowTemporarily(mainProcessName, popup.AllowedDuration);
                                }
                                else
                                {
                                    KillApplication(mainProcessName);
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur dans OnProcessStarted pour {processName}: {ex.Message}");
                }
            });
        }

        // Arrête la surveillance des processus
        public void Stop()
        {
            _watcher?.Stop();
            _watcher?.Dispose();
            _temporaryAllowances.Clear();
        }

        // Active ou désactive le bloqueur avec option de mode focus
        public void SetActive(bool active, bool isFocusMode = false)
        {
            _isActive = active;

            _isFocusModeSessionActive = active && isFocusMode;

            if (!active)
            {
                _temporaryAllowances.Clear();
                _lastGlobalPromptTime = DateTime.MinValue;
            }
        }

        // Vérifie si une application est considérée comme distrayante
        public bool IsDistractingApp(string processName)
        {
            string mainProcessName = GetMainProcessName(processName.ToLower());

            return _blockedApps.Any(blockedApp =>
            {
                var normalizedBlockedApp = NormalizeAppName(blockedApp);

                if (AreNamesAlike(mainProcessName, normalizedBlockedApp))
                {
                    return true;
                }

                if (_relatedProcesses.ContainsKey(normalizedBlockedApp))
                {
                    if (_relatedProcesses[normalizedBlockedApp].Any(relatedProc => AreNamesAlike(mainProcessName, relatedProc)))
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        // Récupère le nom d'affichage d'un processus
        private async Task<string> GetDisplayName(Process process, string fallbackName)
        {
            try
            {
                if (_displayNameCache.TryGetValue(process.ProcessName.ToLower(), out string? cachedName))
                {
                    return cachedName;
                }

                string? displayName = null;
                try
                {
                    if (!string.IsNullOrEmpty(process.MainModule?.FileVersionInfo.FileDescription))
                        displayName = process.MainModule.FileVersionInfo.FileDescription;
                    else if (!string.IsNullOrEmpty(process.MainModule?.FileVersionInfo.ProductName))
                        displayName = process.MainModule.FileVersionInfo.ProductName;
                }
                catch { }

                if (string.IsNullOrEmpty(displayName))
                    displayName = char.ToUpper(fallbackName[0]) + fallbackName[1..];

                _displayNameCache[process.ProcessName.ToLower()] = displayName;
                return displayName;
            }
            catch { return char.ToUpper(fallbackName[0]) + fallbackName[1..]; }
        }

        // Obtient le nom principal d'un processus en tenant compte des processus liés
        private string GetMainProcessName(string processName)
        {
            processName = processName.ToLower();

            foreach (var kvp in _relatedProcesses)
            {
                if (kvp.Value.Any(p =>
                    processName.Contains(p.Replace(".exe", "")) ||
                    p.Replace(".exe", "").Contains(processName)))
                {
                    return kvp.Key;
                }
            }
            return processName;
        }

        // Attend qu'une fenêtre principale soit disponible
        private async Task<bool> WaitForMainWindow(Process process, TimeSpan timeout)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start) < timeout)
            {
                try
                {
                    if (process.HasExited) return false;
                    process.Refresh();
                    if (process.MainWindowHandle != IntPtr.Zero) return true;
                }
                catch { return false; }
                await Task.Delay(100);
            }
            return false;
        }

        // Compare deux noms d'applications en les normalisant
        private bool AreNamesAlike(string name1, string name2)
        {
            string Normalize(string s) => new string(s.ToLower().Where(char.IsLetterOrDigit).ToArray());

            string normalizedName1 = Normalize(name1);
            string normalizedName2 = Normalize(name2);

            if (string.IsNullOrEmpty(normalizedName1) || string.IsNullOrEmpty(normalizedName2))
            {
                return false;
            }

            return normalizedName1.Contains(normalizedName2) || normalizedName2.Contains(normalizedName1);
        }

        // Ferme une application et tous ses processus liés
        public void KillApplication(string mainAppName)
        {
            mainAppName = mainAppName.ToLower();
            List<string> processesToKill = new List<string> { mainAppName };

            if (_relatedProcesses.ContainsKey(mainAppName))
            {
                processesToKill.AddRange(_relatedProcesses[mainAppName]);
            }

            foreach (var processName in processesToKill.Distinct())
            {
                try
                {
                    var exeName = processName.Replace(".exe", "");
                    var processes = Process.GetProcessesByName(exeName);
                    foreach (var process in processes)
                    {
                        try
                        {
                            process.Kill(true);
                            Debug.WriteLine($"Processus {process.ProcessName} (ID: {process.Id}) tué.");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Impossible de tuer le processus {process.ProcessName} (ID: {process.Id}): {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur lors de la recherche du processus {processName}: {ex.Message}");
                }
            }
        }

        private readonly Dictionary<string, List<string>> _relatedProcesses = new()
        {
            { "steam", new List<string> { "steam", "steamwebhelper", "steamservice", "steamclient", "gameoverlayui" } },
            { "epicgameslauncher", new List<string> { "epicgameslauncher", "epicgameslauncherhelper", "epicwebhelper" } },
            { "discord", new List<string> { "discord", "discordptb", "discordcanary", "discordhelper" } },
        };

        private readonly Dictionary<string, string> _appAliases = new()
        {
            { "lol", "league of legends" },
            { "battlenet", "battle.net" },
            { "chrome", "google chrome" },
            { "msedge", "microsoft edge" },
            { "firefox", "mozilla firefox" }
        };
        
        // Charge la liste des applications bloquées depuis les paramètres
        public static List<string> LoadBlockedApps()
        {
            return Settings.Default.BlockedApps
                       .Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(app => app.Trim())
                       .ToList();
        }

        // Dans le fichier ConcenTrade/Timer + AppBlock/AppBlocker.cs

        public void KillProcess(string processName)
        {
            try
            {
                // Le nom du processus peut parfois inclure ou non l'extension .exe
                var exeName = processName.EndsWith(".exe") ? processName.Substring(0, processName.Length - 4) : processName;
                var processes = Process.GetProcessesByName(exeName);

                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit(); // Attendre que le processus soit bien terminé
                    }
                    catch (Exception ex)
                    {
                        // Gérer les cas où l'on n'a pas les droits, etc.
                        Console.WriteLine($"Impossible de fermer le processus {process.ProcessName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur en cherchant le processus {processName}: {ex.Message}");
            }
        }

        public void MinimizeProcess(string mainAppName)
        {
            mainAppName = mainAppName.ToLower();
            List<string> processesToMinimize = new List<string> { mainAppName };

            if (_relatedProcesses.ContainsKey(mainAppName))
            {
                processesToMinimize.AddRange(_relatedProcesses[mainAppName]);
            }

            foreach (var processName in processesToMinimize.Distinct())
            {
                try
                {
                    var exeName = processName.Replace(".exe", "");
                    var processes = Process.GetProcessesByName(exeName);
                    foreach (var process in processes)
                    {
                        if (process.MainWindowHandle != IntPtr.Zero)
                        {
                            try
                            {
                                ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
                                Debug.WriteLine($"Processus {process.ProcessName} (ID: {process.Id}) minimisé.");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Impossible de minimiser le processus {process.ProcessName} (ID: {process.Id}): {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur lors de la recherche du processus {processName} pour minimisation: {ex.Message}");
                }
            }
        }

    }




    public class TemporaryAllowanceEventArgs : EventArgs
    {
        public string ProcessName { get; }
        public TimeSpan Duration { get; }

        public TemporaryAllowanceEventArgs(string processName, TimeSpan duration)
        {
            ProcessName = processName;
            Duration = duration;
        }

    }



}