using System.Windows;
using System.Windows.Controls;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.List
{
    public class ListNode : Node
    {
        public ListNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            var label = new Label {Width = 40, Content = "List", Margin = new Thickness(5), FontSize= 14};

            AddControlToNode(label);

            AddInputPortToNode("Items", typeof (object), true);
            AddOutputPortToNode("List", typeof (object));
        }

        public override void Calculate()
        {
                OutputPorts[0].Data = InputPorts[0].Data;
        }

        public override Node Clone()
        {
            var node = new ListNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };

            return node;
        }
    }
}