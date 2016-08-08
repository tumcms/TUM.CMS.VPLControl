using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TUM.CMS.VplControl.Core
{
    public class NodePinButton : Button
    {
        public NodePinButton(VplElement hostElement)
        {
            HostElement = hostElement;
            //HostElement.HostCanvas.Children.Add(this);
            HostElement.HitTestGrid.Children.Add(this);
            Grid.SetColumn(this, 3);

            Style = FindResource("PinButton20") as Style;

            HostNodeGroup_PropertyChanged(null, null);
            hostElement.PropertyChanged += HostNodeGroup_PropertyChanged;
        }

        public VplElement HostElement { get; set; }

        private void HostNodeGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var hostPosition = HostElement.GetPosition();
            Canvas.SetTop(this, hostPosition.Y - 30);
            Canvas.SetLeft(this, hostPosition.X + HostElement.ActualWidth - 40);
        }
    }
}