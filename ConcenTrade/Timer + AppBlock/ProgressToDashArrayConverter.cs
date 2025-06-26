using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Concentrade
{
    /// <summary>
    /// Convertit une valeur de progression (0-100) en une collection de tirets (StrokeDashArray)
    /// pour dessiner un arc de cercle...
    /// </summary>
    public class ProgressToDashArrayConverter : IMultiValueConverter
    {
        // Convertit une valeur de progression en collection de tirets pour dessiner un arc de cercle
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 4 ||
                !double.TryParse(values[0]?.ToString(), out double progress) ||
                !double.TryParse(values[1]?.ToString(), out double width) ||
                !double.TryParse(values[2]?.ToString(), out double height) ||
                !double.TryParse(values[3]?.ToString(), out double strokeThickness) ||
                values.Any(v => v == DependencyProperty.UnsetValue))
            {
                return new DoubleCollection(new[] { 0.0 });
            }

            // Calcule la circonférence de l'ellipse en se basant sur ses dimensions et l'épaisseur du trait..
            double radiusX = (width - strokeThickness) / 2;
            double radiusY = (height - strokeThickness) / 2;
            // Utilise la formule de Ramanujan pour une approximation précise de la circonférence d'une ellipse.
            double circumference = Math.PI * (3 * (radiusX + radiusY) - Math.Sqrt((3 * radiusX + radiusY) * (radiusX + 3 * radiusY)));

            // Calcule la longueur du tiret visible en fonction du pourcentage de progression.
            double dashLength = (progress / 100.0) * circumference;

            // Retourne une collection avec la longueur du tiret et un espace suffisamment grand pour couvrir le reste.
            return new DoubleCollection(new[] { dashLength, circumference });
        }

        // La conversion inverse n'est pas implémentée
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
