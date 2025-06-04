using System.Configuration;
using System.Data;
using System.Windows;

namespace Concentrade
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AppBlocker AppBlocker { get; private set; }

        public App()
        {
            AppBlocker = new AppBlocker();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppBlocker.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AppBlocker.Stop();
            base.OnExit(e);
        }
    }
}
