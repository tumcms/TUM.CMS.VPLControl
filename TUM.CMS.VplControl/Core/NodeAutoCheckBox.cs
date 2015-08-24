using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TUM.CMS.VplControl.Core
{
    public class NodeAutoCheckBox : CheckBox
    {
        public NodeAutoCheckBox(VplElement hostNodeGroup)
        {
            HostElement = hostNodeGroup;
            HostElement.HostCanvas.Children.Add(this);

            IsChecked = true;
            BorderBrush = Application.Current.Resources["BrushBlue"] as Brush;
            Foreground = Application.Current.Resources["BrushBlue"] as Brush;

            HostNodeGroup_PropertyChanged(null, null);
            HostElement.PropertyChanged += HostNodeGroup_PropertyChanged;
        }

        public VplElement HostElement { get; set; }

        private void HostNodeGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var hostPosition = HostElement.GetPosition();
            Canvas.SetTop(this, hostPosition.Y - 23);
            Canvas.SetLeft(this, hostPosition.X + HostElement.ActualWidth - 55);
        }
    }
}