using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TUM.CMS.VplControl.Core
{
    public class NodeCaptionLabel : Label
    {
        public NodeCaptionLabel(VplElement hostElement)
        {
            HostElement = hostElement;
            HostElement.HostCanvas.Children.Add(this);

            Style = FindResource("EditableLabelStyleGroup") as Style;


            var b = new Binding("Content")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            };
            HostElement.SetBinding(VplElement.nodeGroupNameProperty, b);


            HostNodeGroup_PropertyChanged(null, null);
            HostElement.PropertyChanged += HostNodeGroup_PropertyChanged;
        }

        public VplElement HostElement { get; set; }

        private void HostNodeGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var hostPosition = HostElement.GetPosition();
            Canvas.SetTop(this, hostPosition.Y - 28);
            Canvas.SetLeft(this, hostPosition.X + 40);

            if (HostElement.Width - 60 > 0)
                MaxWidth = HostElement.Width - 60;
        }
    }
}