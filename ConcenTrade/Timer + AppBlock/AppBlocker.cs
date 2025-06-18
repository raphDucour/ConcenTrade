using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

namespace Concentrade
{
    public class AppBlocker
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MINIMIZE = 6;

        private ManagementEventWatcher? _watcher;
        private readonly object _promptLock = new object();
        private bool _isActive = false;

        // --- MODIFIÉ ---
        // La liste est maintenant une List<string> et n'est plus initialisée depuis les Settings.
        private List<string> _blockedApps = new List<string>();

        private readonly Dictionary<string, string> _displayNameCache = new();

        // --- MODIFIÉ ---
        // Cooldown global pour éviter les popups multiples de différents processus.
        private DateTime _lastGlobalPromptTime = DateTime.MinValue;
        private readonly TimeSpan _popupCooldown = TimeSpan.FromSeconds(15); // Augmenté pour être sûr

        private readonly Dictionary<string, DateTime> _temporaryAllowances = new();

        public event EventHandler<TemporaryAllowanceEventArgs>? OnTemporaryAllowance;

        public void AllowTemporarily(string processName, TimeSpan duration)
        {
            var mainProcessName = GetMainProcessName(processName);
            _temporaryAllowances[mainProcessName] = DateTime.Now.Add(duration);
            OnTemporaryAllowance?.Invoke(this, new TemporaryAllowanceEventArgs(mainProcessName, duration));
        }

        // --- MODIFIÉ ---
        // Le constructeur est maintenant vide. La liste des applications bloquées
        // sera chargée depuis l'extérieur après la connexion de l'utilisateur.
        public AppBlocker()
        {
        }

        // --- MODIFIÉ ---
        // Accepte IEnumerable<string> pour plus de flexibilité et ne sauvegarde plus rien.
        public void UpdateBlockedApps(IEnumerable<string> newBlockedApps)
        {
            _blockedApps = new List<string>(newBlockedApps.Select(app => NormalizeAppName(app)));
            _displayNameCache.Clear(); // Vider le cache quand la liste change
        }

        // --- SUPPRIMÉ ---
        // La méthode SaveBlockedApps() a été complètement supprimée.

        // --- MODIFIÉ ---
        // Renvoie maintenant une List<string>.
        public List<string> GetBlockedApps()
        {
            return _blockedApps;
        }

        private string NormalizeAppName(string appName)
        {
            appName = appName.ToLower().Trim();

            if (_appAliases.TryGetValue(appName, out string? aliasedName))
            {
                return aliasedName;
            }

            return appName;
        }

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

                        // On vérifie le cooldown une première fois pour éviter du travail inutile.
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

                        // Si aucune fenêtre n'est prête, on quitte SANS activer le cooldown.
                        if (!windowReady && !isGame) return;

                        if (process.MainWindowHandle != IntPtr.Zero)
                        {
                            Application.Current.Dispatcher.Invoke(() => ShowWindow(process.MainWindowHandle, SW_MINIMIZE));
                            await Task.Delay(100);
                        }

                        string displayName = await GetDisplayName(process, mainProcessName);

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            // CHANGEMENT ICI : Le verrou et la mise à jour du cooldown sont déplacés ici.
                            lock (_promptLock)
                            {
                                // On revérifie le cooldown une dernière fois pour être sûr.
                                if ((DateTime.Now - _lastGlobalPromptTime) < _popupCooldown)
                                {
                                    return;
                                }
                                // Le cooldown est activé SEULEMENT MAINTENANT, juste avant d'afficher le popup.
                                _lastGlobalPromptTime = DateTime.Now;
                            }

                            if (process.MainWindowHandle != IntPtr.Zero)
                            {
                                ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
                            }

                            var popup = new BlocagePopup(displayName)
                            {
                                Topmost = true,
                                WindowStartupLocation = WindowStartupLocation.CenterScreen
                            };

                            popup.Activate();
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
                    Debug.WriteLine($"Erreur dans OnProcessStarted pour {processName}: {ex.Message}");
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
                // --- MODIFIÉ ---
                // Réinitialise le cooldown global.
                _lastGlobalPromptTime = DateTime.MinValue;
            }
        }

        public bool IsDistractingApp(string processName)
        {
            string mainProcessName = GetMainProcessName(processName.ToLower());

            return _blockedApps.Any(blockedApp =>
            {
                // On utilise toujours NormalizeAppName pour gérer les alias (ex: "lol" -> "league of legends")
                var normalizedBlockedApp = NormalizeAppName(blockedApp);

                // On utilise notre nouvelle logique de comparaison plus intelligente
                if (AreNamesAlike(mainProcessName, normalizedBlockedApp))
                {
                    return true;
                }

                // On conserve la logique pour les processus liés (ex: pour Discord, Steam, etc.)
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
                catch { /* Ignorer les erreurs d'accès */ }

                if (string.IsNullOrEmpty(displayName))
                    displayName = char.ToUpper(fallbackName[0]) + fallbackName[1..];

                _displayNameCache[process.ProcessName.ToLower()] = displayName;
                return displayName;
            }
            catch { return char.ToUpper(fallbackName[0]) + fallbackName[1..]; }
        }

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

        private bool AreNamesAlike(string name1, string name2)
        {
            // Fonction interne pour normaliser une chaîne de caractères :
            // - Met tout en minuscules.
            // - Ne garde que les lettres et les chiffres.
            // Par exemple, "My Super Game" devient "mysupergame" et "my_super_game" devient aussi "mysupergame".
            string Normalize(string s) => new string(s.ToLower().Where(char.IsLetterOrDigit).ToArray());

            string normalizedName1 = Normalize(name1);
            string normalizedName2 = Normalize(name2);

            if (string.IsNullOrEmpty(normalizedName1) || string.IsNullOrEmpty(normalizedName2))
            {
                return false;
            }

            // Vérifie si l'un des noms normalisés contient l'autre.
            return normalizedName1.Contains(normalizedName2) || normalizedName2.Contains(normalizedName1);
        }

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
            { "valorant", new List<string> {
                "valorant", "valorant-win64-shipping", "riot client",
                "riotclientservices", "vgc", "riotclientux"
            } }
        };

        private readonly Dictionary<string, string> _appAliases = new()
        {
            { "lol", "league of legends" },
            { "battlenet", "battle.net" },
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