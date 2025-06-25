using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
                SpotlightCanvas.Children.Clear();
                return;
            }
            SpotlightCanvas.Children.Clear();
            if (_targets[_step] != null)
            {
                // Calculer la position du bouton ciblé
                var target = _targets[_step] as FrameworkElement;
                if (target != null && target.IsVisible)
                {
                    // Obtenir la position du bouton par rapport à la fenêtre principale
                    var relativePoint = target.TransformToAncestor(Window.GetWindow(this)).Transform(new Point(0, 0));
                    double centerX = relativePoint.X + target.ActualWidth / 2;
                    double centerY = relativePoint.Y + target.ActualHeight / 2;
                    double radius = Math.Max(target.ActualWidth, target.ActualHeight) / 2 + 30; // marge autour du bouton

                    // Overlay sombre
                    var overlay = new Rectangle
                    {
                        Width = Window.GetWindow(this).ActualWidth,
                        Height = Window.GetWindow(this).ActualHeight,
                        Fill = new SolidColorBrush(Color.FromArgb(180, 20, 20, 30))
                    };

                    // Trou circulaire (spotlight)
                    var geometryGroup = new GeometryGroup();
                    geometryGroup.Children.Add(new RectangleGeometry(new Rect(0, 0, overlay.Width, overlay.Height)));
                    geometryGroup.Children.Add(new EllipseGeometry(new Point(centerX, centerY), radius, radius));
                    geometryGroup.FillRule = FillRule.EvenOdd;
                    var path = new Path
                    {
                        Data = geometryGroup,
                        Fill = overlay.Fill,
                        Opacity = 1,
                        IsHitTestVisible = false
                    };
                    SpotlightCanvas.Children.Add(path);
                }
            }
            if (_targets[_step] == null)
            {
                TutorialPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
                TutorialPopup.PlacementTarget = null;
            }
            else
            {
                TutorialPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                TutorialPopup.PlacementTarget = _targets[_step];
            }
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