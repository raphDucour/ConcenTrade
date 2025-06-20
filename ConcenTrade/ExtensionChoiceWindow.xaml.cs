using System;
using System.Windows;
using System.Windows.Controls;

namespace ConcenTrade
{
    public partial class ExtensionChoiceWindow : Window
    {
        public TimeSpan ExtensionDuration { get; private set; } = TimeSpan.Zero;

        public ExtensionChoiceWindow(string processName)
        {
            InitializeComponent();
            MessageTextBlock.Text = $"Souhaitez-vous prolonger l'autorisation pour {processName} ?";
        }

        private void Extend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int minutes))
            {
                ExtensionDuration = TimeSpan.FromMinutes(minutes);
                DialogResult = true;
                Close();
            }
        }

        private void Decline_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}