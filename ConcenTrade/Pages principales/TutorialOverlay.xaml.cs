using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Concentrade.Pages_principales
{
    public partial class TutorialOverlay : UserControl
    {
        private int _step = 0;
        private UIElement[] _targets;
        private string[] _texts;
        private ScaleTransform _currentPulseTransform = null;
        private FrameworkElement _lastTarget = null;

        public bool IsOpen
        {
            get { return TutorialPopup.IsOpen; }
            set
            {
                TutorialPopup.IsOpen = value;
                this.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                // Permettre les interactions seulement quand le tutoriel est actif
                this.IsHitTestVisible = value;
            }
        }

        public TutorialOverlay()
        {
            InitializeComponent();
            // S'assurer que le tutoriel est fermé au démarrage
            this.Visibility = Visibility.Collapsed;
            this.IsHitTestVisible = false;
            TutorialPopup.IsOpen = false;
        }

        public void StartTutorial(UIElement[] targets, string[] texts)
        {
            _targets = targets;
            _texts = texts;
            _step = 0;
            
            // S'assurer que le tutoriel est visible et actif
            this.Visibility = Visibility.Visible;
            this.IsHitTestVisible = true;
            
            ShowStep();
        }

        private void ShowStep()
        {
            if (_lastTarget != null && _currentPulseTransform != null)
            {
                _lastTarget.RenderTransform = null;
                _lastTarget.RenderTransformOrigin = new Point(0.5, 0.5);
                _currentPulseTransform = null;
            }

            if (_targets == null || _texts == null || _step >= _targets.Length)
            {
                IsOpen = false;
                IntroOverlay.Visibility = Visibility.Collapsed;
                return;
            }
            if (_targets[_step] == null)
            {
                IntroText.Text = _texts[_step];
                IntroOverlay.Visibility = Visibility.Visible;
                TutorialPopup.IsOpen = false;
            }
            else
            {
                IntroOverlay.Visibility = Visibility.Collapsed;
                if (_targets[_step] is TextBlock tb && tb.Name == "CycleCountLabel")
                {
                    TutorialPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    TutorialPopup.VerticalOffset = 12;
                    TutorialPopup.HorizontalOffset = 0;
                }
                else
                {
                    TutorialPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                    TutorialPopup.HorizontalOffset = 0;
                    TutorialPopup.VerticalOffset = 0;
                }
                TutorialPopup.PlacementTarget = _targets[_step];
                TutorialText.Text = _texts[_step];
                IsOpen = true;

                var target = _targets[_step] as FrameworkElement;
                if (target != null)
                {
                    var scale = new ScaleTransform(1, 1);
                    target.RenderTransform = scale;
                    target.RenderTransformOrigin = new Point(0.5, 0.5);
                    _currentPulseTransform = scale;
                    _lastTarget = target;

                    var pulse = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 1.15,
                        Duration = TimeSpan.FromMilliseconds(400),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever,
                        EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                    };
                    scale.BeginAnimation(ScaleTransform.ScaleXProperty, pulse);
                    scale.BeginAnimation(ScaleTransform.ScaleYProperty, pulse);
                }

                if (_step == 1 && _targets[_step] is RadioButton pomodoroButton)
                {
                    pomodoroButton.IsChecked = true;
                }
                if (_step == 3)
                {
                    var menuPage = Window.GetWindow(this)?.Content as MenuPage;
                    menuPage?.StartSession_Click(null, null);
                }
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
                if (_lastTarget != null && _currentPulseTransform != null)
                {
                    _lastTarget.RenderTransform = null;
                    _lastTarget.RenderTransformOrigin = new Point(0.5, 0.5);
                    _currentPulseTransform = null;
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