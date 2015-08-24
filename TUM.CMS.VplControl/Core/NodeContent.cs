using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TUM.CMS.VplControl.Core
{
    public class NodeContent : Canvas
    {
        public NodeContent()
        {
            MinWidth = 100;
            MinHeight = 50;
            Margin = new Thickness(5);
            Background = Brushes.Beige;
            MouseDown += HandleContentMouseDown;
        }

        public void HandleContentMouseDown(object sender, MouseButtonEventArgs args)
        {
        }
    }
}