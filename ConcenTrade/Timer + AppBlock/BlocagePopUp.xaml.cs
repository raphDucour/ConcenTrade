using System.Windows;

namespace Concentrade
{
    public partial class BlocagePopup : Window
    {
        public bool ContinueAnyway { get; private set; } = false;

        public BlocagePopup(string appName)
        {
            InitializeComponent();
            MessageText.Text = $"{appName} est bloqué pendant ta session.\nSouhaites-tu vraiment l'ouvrir ?";
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            ContinueAnyway = true;
            this.DialogResult = true;
            Close();
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            ContinueAnyway = false;
            this.DialogResult = false;
            Close();
        }
    }
}
