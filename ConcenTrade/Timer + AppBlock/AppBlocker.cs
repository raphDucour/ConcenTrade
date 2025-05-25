using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows;

namespace Concentrade
{
    public class AppBlocker
    {
        private ManagementEventWatcher? _watcher;

        // Liste des applis à bloquer
        private readonly string[] _blockedApps = { "discord.exe", "spotify.exe", "tiktok.exe" };

        // Cooldown par application
        private readonly Dictionary<string, DateTime> _lastPromptTime = new();
        private readonly TimeSpan _popupCooldown = TimeSpan.FromSeconds(10);

        public void Start()
        {
            _watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _watcher.EventArrived += async (s, e) =>
            {
                string? processName = e.NewEvent.Properties["ProcessName"].Value?.ToString()?.ToLower();
                int processId = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);

                if (processName != null && _blockedApps.Contains(processName))
                {
                    if (_lastPromptTime.TryGetValue(processName, out DateTime lastTime))
                    {
                        if ((DateTime.Now - lastTime) < _popupCooldown)
                            return;
                    }

                    try
                    {
                        var process = Process.GetProcessById(processId);
                        bool windowReady = await WaitForMainWindow(process, TimeSpan.FromSeconds(10));

                        if (!windowReady) return; // La fenêtre n’est jamais apparue

                        _lastPromptTime[processName] = DateTime.Now;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var popup = new BlocagePopup(processName);
                            bool? result = popup.ShowDialog();

                            if (popup.ContinueAnyway == false)
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

        // Attend que la fenêtre principale soit disponible
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

        public void Stop()
        {
            _watcher?.Stop();
            _watcher?.Dispose();
        }

        public bool IsDistractingApp(string processName)
        {
            string[] blockedApps = new[]
            {
                "discord",
                "spotify",
                "edge",
                "opera",
                "teams",
                "epicgameslauncher",
                "steam"
            };

            return Array.Exists(blockedApps, name => processName.ToLower().Contains(name));
        }
    }
}
