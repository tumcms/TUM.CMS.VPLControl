using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TUM.CMS.VplControl.Controls
{
    public class ExpanderColor : Expander
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color",
                typeof (Color), typeof (ExpanderColor)); // optionally metadata for defaults etc.

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush",
                typeof (SolidColorBrush), typeof (ExpanderColor)); // optionally metadata for defaults etc.

        public Color Color
        {
            get { return (Color) GetValue(ColorProperty); }
            set
            {
                SetValue(ColorProperty, value);
                SetValue(BrushProperty, new SolidColorBrush(value));
            }
        }

        public SolidColorBrush Brush
        {
            get { return new SolidColorBrush((Color) GetValue(ColorProperty)); }
            set { SetValue(ColorProperty, value.Color); }
        }
    }
}