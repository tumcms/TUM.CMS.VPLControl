using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TUM.CMS.VplControl.Core
{
    public class NodeBinButton : Button
    {
        public NodeBinButton(VplElement hostNodeGroup)
        {
            HostElement = hostNodeGroup;
            HostElement.HostCanvas.Children.Add(this);

            Style = FindResource("BinButton20") as Style;

            HostNodeGroup_PropertyChanged(null, null);
            HostElement.PropertyChanged += HostNodeGroup_PropertyChanged;
        }

        public VplElement HostElement { get; set; }

        private void HostNodeGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var hostPosition = HostElement.GetPosition();
            Canvas.SetTop(this, hostPosition.Y - 30);
            Canvas.SetLeft(this, hostPosition.X + HostElement.ActualWidth - 15);
        }
    }
}