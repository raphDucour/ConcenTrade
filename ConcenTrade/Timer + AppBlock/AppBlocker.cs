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
        private HashSet<string> _alreadyPrompted = new HashSet<string>();


        // Liste des applis à bloquer
        private readonly string[] _blockedApps = { "discord.exe", "spotify.exe", "tiktok.exe" };

        public void Start()
        {
            _watcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            _watcher.EventArrived += (s, e) =>
            {
                string? processName = e.NewEvent.Properties["ProcessName"].Value?.ToString()?.ToLower();
                int processId = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);

                if (processName != null && _blockedApps.Contains(processName))
                {
               

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
