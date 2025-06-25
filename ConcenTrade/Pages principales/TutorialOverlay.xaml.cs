using System.Windows;
using System.Windows.Controls;

namespace Concentrade.Pages_principales
{
    public partial class TutorialOverlay : UserControl
    {
        private int _step = 0;
        private UIElement[] _targets;
        private string[] _texts;

        public bool IsOpen
        {
            get { return TutorialPopup.IsOpen; }
            set { TutorialPopup.IsOpen = value; }
        }

        public TutorialOverlay()
        {
            InitializeComponent();
        }

        public void StartTutorial(UIElement[] targets, string[] texts)
        {
            _targets = targets;
            _texts = texts;
            _step = 0;
            ShowStep();
        }

        private void ShowStep()
        {
            if (_targets == null || _texts == null || _step >= _targets.Length)
            {
                IsOpen = false;
                return;
            }
            TutorialPopup.PlacementTarget = _targets[_step];
            TutorialText.Text = _texts[_step];
            IsOpen = true;
        }

        private void NextTutorialStep_Click(object sender, RoutedEventArgs e)
        {
            _step++;
            if (_step < _targets.Length)
            {
                ShowStep();
            }
            else
            {
                IsOpen = false;
            }
        }
    }
} 