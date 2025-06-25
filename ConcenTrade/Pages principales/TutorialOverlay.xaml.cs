using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Input;

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
            set
            {
                TutorialPopup.IsOpen = value;
                this.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
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
                IntroOverlay.Visibility = Visibility.Collapsed;
                return;
            }
            if (_targets[_step] == null)
            {
                // Affiche l'intro overlay centré
                IntroText.Text = _texts[_step];
                IntroOverlay.Visibility = Visibility.Visible;
                TutorialPopup.IsOpen = false;
            }
            else
            {
                IntroOverlay.Visibility = Visibility.Collapsed;
                TutorialPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                TutorialPopup.PlacementTarget = _targets[_step];
                TutorialPopup.HorizontalOffset = 0;
                TutorialPopup.VerticalOffset = 0;
                TutorialText.Text = _texts[_step];
                IsOpen = true;
            }
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

        private void MainWindow_SizeChanged_CenterPopup(object sender, SizeChangedEventArgs e)
        {
            if (_targets != null && _step < _targets.Length && _targets[_step] == null && TutorialPopup.IsOpen)
            {
                var mainWindow = Window.GetWindow(this);
                if (mainWindow != null)
                {
                    TutorialPopup.HorizontalOffset = (mainWindow.ActualWidth - TutorialPopup.ActualWidth) / 2;
                    TutorialPopup.VerticalOffset = (mainWindow.ActualHeight - TutorialPopup.ActualHeight) / 2;
                }
            }
        }

        private void IntroNext_Click(object sender, RoutedEventArgs e)
        {
            _step++;
            ShowStep();
        }
    }
} 