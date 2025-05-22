using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Windows;

namespace Concentrade
{
    public class AppBlocker
    {
        private ManagementEventWatcher? _watcher;

        // Liste des applis à bloquer
        private readonly string[] _blockedApps = { "chrome.exe", "discord.exe", "spotify.exe", "tiktok.exe" };

        public void Start()
        {
            _watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _watcher.EventArrived += (s, e) =>
            {
                string? processName = e.NewEvent.Properties["ProcessName"].Value?.ToString()?.ToLower();

                if (processName != null && _blockedApps.Contains(processName))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(
                            $"⛔ {processName} est bloqué pendant ta session.\nReste concentré 💪",
                            "ConcenTrade - Blocage",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );

                        foreach (var proc in Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(processName)))
                        {
                            try { proc.Kill(); } catch { }
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
    }
}
