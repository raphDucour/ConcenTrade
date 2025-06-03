using System;
using System.Windows;

namespace Concentrade
{
    public partial class TimeUpPopup : Window
    {
        public enum TimeUpAction
        {
            Prolonger,
            Reprendre,
            Fermer
        }

        public TimeUpAction Action { get; private set; }
        public string AppName { get; private set; }

        public TimeUpPopup(string appName)
        {
            InitializeComponent();
            AppName = appName;
        }

        private void Prolonger_Click(object sender, RoutedEventArgs e)
        {
            Action = TimeUpAction.Prolonger;
            DialogResult = true;
            Close();
        }

        private void Reprendre_Click(object sender, RoutedEventArgs e)
        {
            Action = TimeUpAction.Reprendre;
            DialogResult = true;
            Close();
        }

        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            Action = TimeUpAction.Fermer;
            DialogResult = true;
            Close();
        }
    }
} 