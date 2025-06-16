using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.IO;
using Concentrade.Properties;

namespace Concentrade
{
    public class AppBlocker
    {
        private ManagementEventWatcher? _watcher;
        private readonly object _promptLock = new object();
        private bool _isActive = false;

        // Liste modifiable des applications à bloquer
        private string[] _blockedApps;

        // Cache des noms d'affichage des applications
        private readonly Dictionary<string, string> _displayNameCache = new();

        // Cooldown par application
        private readonly Dictionary<string, DateTime> _lastPromptTime = new();
        private readonly TimeSpan _popupCooldown = TimeSpan.FromSeconds(10);

        // Autorisations temporaires
        private readonly Dictionary<string, DateTime> _temporaryAllowances = new();
        
        // Event pour notifier quand une application est temporairement autorisée
        public event EventHandler<TemporaryAllowanceEventArgs>? OnTemporaryAllowance;

        // Méthode pour déclencher l'événement
        public void AllowTemporarily(string processName, TimeSpan duration)
        {
            var mainProcessName = GetMainProcessName(processName);
            _temporaryAllowances[mainProcessName] = DateTime.Now.Add(duration);
            OnTemporaryAllowance?.Invoke(this, new TemporaryAllowanceEventArgs(mainProcessName, duration));
        }

        public AppBlocker()
        {
            // Charger la liste depuis les paramètres ou utiliser la liste par défaut
            var savedApps = Settings.Default.BlockedApps;
            if (!string.IsNullOrEmpty(savedApps))
            {
                _blockedApps = savedApps.Split(',');
            }
            else
            {
                // Liste par défaut
                _blockedApps = new[]
                { 
                    "discord",
                    "spotify",
                    "tiktok",
                    "chrome",
                    "msedge",
                    "firefox",
                    "opera",
                    "steam",
                    "epicgameslauncher",
                    "valorant"
                };
                // Sauvegarder la liste par défaut
                SaveBlockedApps();
            }
        }

        public void UpdateBlockedApps(string[] newBlockedApps)
        {
            _blockedApps = newBlockedApps.Select(app => NormalizeAppName(app)).ToArray();
            _displayNameCache.Clear(); // Vider le cache quand la liste change
            SaveBlockedApps(); // Sauvegarder les modifications
        }

        private void SaveBlockedApps()
        {
            Settings.Default.BlockedApps = string.Join(",", _blockedApps);
            Settings.Default.Save();
        }

        public string[] GetBlockedApps()
        {
            return _blockedApps;
        }

        private string NormalizeAppName(string appName)
        {
            appName = appName.ToLower().Trim();
            
            // Vérifier les alias
            if (_appAliases.TryGetValue(appName, out string? aliasedName))
            {
                return aliasedName;
            }

            return appName;
        }

        public void Start()
        {
            _watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _watcher.EventArrived += OnProcessStarted;
            _watcher.Start();
        }

        private void OnProcessStarted(object sender, EventArrivedEventArgs e)
        {
            if (!_isActive) return;

            string? processName = e.NewEvent.Properties["ProcessName"].Value?.ToString();
            if (string.IsNullOrEmpty(processName)) return;

            // Démarrer le traitement de manière asynchrone sans bloquer
            Task.Run(async () =>
            {
                try
                {
                    string originalProcessName = Path.GetFileNameWithoutExtension(processName).ToLower();
                    int processId = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);

                    if (IsDistractingApp(originalProcessName))
                    {
                        var mainProcessName = GetMainProcessName(originalProcessName);
                        Process? process = null;
                        try
                        {
                            process = Process.GetProcessById(processId);
                        }
                        catch
                        {
                            return; // Le processus a peut-être déjà été fermé
                        }


                        // --- DÉBUT DE LA SECTION CRITIQUE ---
                        lock (_promptLock)
                        {
                            // On vérifie le cooldown A L'INTÉRIEUR du verrou pour éviter les race conditions
                            if (_lastPromptTime.TryGetValue(mainProcessName, out DateTime lastTime))
                            {
                                if ((DateTime.Now - lastTime) < _popupCooldown)
                                {
                                    return; // Un autre processus de la même app a été traité récemment, on ignore celui-ci.
                                }
                            }
                            // Si on continue, on met à jour le temps immédiatement pour bloquer les suivants.
                            _lastPromptTime[mainProcessName] = DateTime.Now;
                        }
                        // --- FIN DE LA SECTION CRITIQUE ---

                        // Vérifier si l'autorisation temporaire est toujours active
                        if (_temporaryAllowances.TryGetValue(mainProcessName, out DateTime expirationTime))
                        {
                            if (DateTime.Now < expirationTime) return;
                            _temporaryAllowances.Remove(mainProcessName);
                        }

                        bool isGame = mainProcessName.Contains("game") || mainProcessName.Contains("shipping") || _relatedProcesses.ContainsKey(mainProcessName);
                        bool windowReady = isGame || await WaitForMainWindow(process, TimeSpan.FromSeconds(5));
                        if (!windowReady && !isGame) return;

                        string displayName = await GetDisplayName(process, mainProcessName);

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            var popup = new BlocagePopup(displayName);
                            bool? dialogResult = popup.ShowDialog();

                            if (dialogResult == true && popup.ContinueAnyway)
                            {
                                if (popup.TemporarilyAllowed)
                                {
                                    AllowTemporarily(mainProcessName, popup.AllowedDuration);
                                }
                            }
                            else
                            {
                                KillApplication(mainProcessName);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erreur dans le traitement du processus {processName}: {ex.Message}");
                }
            });
        }

        public void Stop()
        {
            _watcher?.Stop();
            _watcher?.Dispose();
            _temporaryAllowances.Clear();
        }

        public void SetActive(bool active)
        {
            _isActive = active;
            if (!active)
            {
                _temporaryAllowances.Clear();
                _lastPromptTime.Clear();
            }
        }

        public bool IsDistractingApp(string processName)
        {
            processName = processName.ToLower();
            var mainProcessName = GetMainProcessName(processName);

            return _blockedApps.Any(blockedApp =>
            {
                var normalizedBlockedApp = NormalizeAppName(blockedApp);
                return mainProcessName == normalizedBlockedApp ||
                       mainProcessName.Contains(normalizedBlockedApp) ||
                       normalizedBlockedApp.Contains(mainProcessName) ||
                       (_relatedProcesses.ContainsKey(normalizedBlockedApp) &&
                        _relatedProcesses[normalizedBlockedApp].Any(p => 
                            mainProcessName.Contains(p.ToLower()) || 
                            p.ToLower().Contains(mainProcessName)));
            });
        }

        private async Task<string> GetDisplayName(Process process, string fallbackName)
        {
            try
            {
                // Vérifier d'abord le cache
                if (_displayNameCache.TryGetValue(process.ProcessName.ToLower(), out string? cachedName))
                {
                    return cachedName;
                }

                // Essayer d'obtenir le nom du fichier de l'application
                string? displayName = null;

                try
                {
                    if (!string.IsNullOrEmpty(process.MainModule?.FileVersionInfo.FileDescription))
                    {
                        displayName = process.MainModule.FileVersionInfo.FileDescription;
                    }
                    else if (!string.IsNullOrEmpty(process.MainModule?.FileVersionInfo.ProductName))
                    {
                        displayName = process.MainModule.FileVersionInfo.ProductName;
                    }
                }
                catch { }

                // Si on n'a pas trouvé de nom, utiliser le nom du processus
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = char.ToUpper(fallbackName[0]) + fallbackName[1..];
                }

                // Mettre en cache
                _displayNameCache[process.ProcessName.ToLower()] = displayName;

                return displayName;
            }
            catch
            {
                return char.ToUpper(fallbackName[0]) + fallbackName[1..];
            }
        }

        private string GetMainProcessName(string processName)
        {
            processName = processName.ToLower();

            // Vérifier les processus liés pour les applications spéciales
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

        private async Task<bool> WaitForMainWindow(Process process, TimeSpan timeout)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start) < timeout)
            {
                try
                {
                    if (process.HasExited) return false;
                    process.Refresh();
                    if (process.MainWindowHandle != IntPtr.Zero)
                        return true;
                }
                catch { return false; }

                await Task.Delay(100);
            }
            return false;
        }

        public void KillApplication(string mainAppName)
        {
            mainAppName = mainAppName.ToLower();
            List<string> processesToKill = new List<string> { mainAppName };

            // Si l'application a des processus liés connus, on les ajoute à la liste
            if (_relatedProcesses.ContainsKey(mainAppName))
            {
                processesToKill.AddRange(_relatedProcesses[mainAppName]);
            }

            // On parcourt tous les noms de processus à tuer
            foreach (var processName in processesToKill.Distinct())
            {
                try
                {
                    // GetProcessesByName attend le nom sans l'extension .exe
                    var exeName = processName.Replace(".exe", "");
                    var processes = Process.GetProcessesByName(exeName);
                    foreach (var process in processes)
                    {
                        try
                        {
                            // On tue le processus et tous ses sous-processus
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

        // Liste des processus liés à bloquer (pour les applications spéciales)
        private readonly Dictionary<string, List<string>> _relatedProcesses = new()
        {
            { "steam", new List<string> { "steam", "steamwebhelper", "steamservice", "steamclient", "gameoverlayui" } },
            { "epicgameslauncher", new List<string> { "epicgameslauncher", "epicgameslauncherhelper", "epicwebhelper" } },
            { "discord", new List<string> { "discord", "discordptb", "discordcanary", "discordhelper" } },
            { "valorant", new List<string> { 
                "valorant", 
                "valorant-win64-shipping", 
                "riot client", 
                "riotclientservices",
                "vgc",
                "riotclientux"
            } }
        };

        // Liste des alias d'applications (noms alternatifs)
        private readonly Dictionary<string, string> _appAliases = new()
        {
            { "valorant", "riot" },
            { "lol", "league of legends" },
            { "battlenet", "battle.net" },
            { "riot", "valorant" },
            { "chrome", "google chrome" },
            { "msedge", "microsoft edge" },
            { "firefox", "mozilla firefox" }
        };
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
