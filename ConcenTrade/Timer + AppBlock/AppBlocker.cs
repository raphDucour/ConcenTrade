using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;

namespace Concentrade
{
    public class AppBlocker
    {
        private ManagementEventWatcher? _watcher;
        private bool _isActive = false;

        // Liste modifiable des applications à bloquer
        private string[] _blockedApps;

        // Cooldown par application
        private readonly Dictionary<string, DateTime> _lastPromptTime = new();
        private readonly TimeSpan _popupCooldown = TimeSpan.FromSeconds(10);

        // Autorisations temporaires
        private readonly Dictionary<string, DateTime> _temporaryAllowances = new();
        
        // Event pour notifier quand une application est temporairement autorisée
        public event EventHandler<TemporaryAllowanceEventArgs>? OnTemporaryAllowance;

        public AppBlocker()
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
                "epicgameslauncher"
            };
        }

        public void UpdateBlockedApps(string[] newBlockedApps)
        {
            _blockedApps = newBlockedApps;
        }

        public string[] GetBlockedApps()
        {
            return _blockedApps;
        }

        public void Start()
        {
            _watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _watcher.EventArrived += async (s, e) =>
            {
                if (!_isActive) return; // Ne rien faire si le blocage n'est pas actif

                string? processName = e.NewEvent.Properties["ProcessName"].Value?.ToString()?.ToLower();
                int processId = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);

                if (processName != null && IsDistractingApp(processName))
                {
                    // Vérifier si l'application est temporairement autorisée
                    if (_temporaryAllowances.TryGetValue(processName, out DateTime expirationTime))
                    {
                        if (DateTime.Now < expirationTime)
                        {
                            // L'application est encore autorisée
                            return;
                        }
                        else
                        {
                            // L'autorisation a expiré, la supprimer
                            _temporaryAllowances.Remove(processName);
                        }
                    }

                    if (_lastPromptTime.TryGetValue(processName, out DateTime lastTime))
                    {
                        if ((DateTime.Now - lastTime) < _popupCooldown)
                            return;
                    }

                    try
                    {
                        var process = Process.GetProcessById(processId);
                        bool windowReady = await WaitForMainWindow(process, TimeSpan.FromSeconds(10));

                        if (!windowReady) return;

                        _lastPromptTime[processName] = DateTime.Now;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var popup = new BlocagePopup(processName);
                            bool? result = popup.ShowDialog();

                            if (result == true && popup.ContinueAnyway)
                            {
                                if (popup.TemporarilyAllowed)
                                {
                                    // Ajouter l'autorisation temporaire
                                    _temporaryAllowances[processName] = DateTime.Now.Add(popup.AllowedDuration);
                                    
                                    // Notifier de l'autorisation temporaire
                                    OnTemporaryAllowance?.Invoke(this, new TemporaryAllowanceEventArgs(
                                        processName, 
                                        popup.AllowedDuration
                                    ));
                                }
                            }
                            else
                            {
                                foreach (var proc in Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(processName)))
                                {
                                    try { proc.Kill(); } catch { }
                                }
                            }
                        });
                    }
                    catch
                    {
                        // Le processus a peut-être été fermé avant qu'on agisse
                    }
                }
            };
            _watcher.Start();
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
            // Convertir en minuscules pour une comparaison insensible à la casse
            processName = processName.ToLower();
            
            // Vérifier si le nom du processus contient l'un des noms bloqués
            return _blockedApps.Any(blockedApp => 
                processName.Contains(blockedApp) || 
                processName == blockedApp || 
                processName == blockedApp + ".exe");
        }

        public void Stop()
        {
            _watcher?.Stop();
            _watcher?.Dispose();
            _temporaryAllowances.Clear();
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

                await Task.Delay(500);
            }
            return false;
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
