using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Windows;

namespace Concentrade
{
    public class AppBlocker
    {
        private ManagementEventWatcher? _watcher;

        // Liste des applis à bloquer
        private readonly string[] _blockedApps = { "discord.exe", "spotify.exe", "tiktok.exe" };

        // Cooldown de popup par appli
        private readonly Dictionary<string, DateTime> _lastPromptTime = new();

        // Durée de blocage (en secondes) avant de réafficher une popup pour la même appli
        private readonly TimeSpan _popupCooldown = TimeSpan.FromSeconds(10);

        public void Start()
        {
            _watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _watcher.EventArrived += (s, e) =>
            {
                string? processName = e.NewEvent.Properties["ProcessName"].Value?.ToString()?.ToLower();
                int processId = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);

                if (processName != null && _blockedApps.Contains(processName))
                {
                    // Si on a déjà affiché une popup récemment pour cette appli, on ignore
                    if (_lastPromptTime.TryGetValue(processName, out DateTime lastTime))
                    {
                        if ((DateTime.Now - lastTime) < _popupCooldown)
                            return;
                    }

                    // Marquer le moment de la dernière popup
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
            };
            _watcher.Start();
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
