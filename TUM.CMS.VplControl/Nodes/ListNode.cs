using System;
using System.Windows;
using System.Windows.Controls;

namespace TUM.CMS.VplControl.Nodes
{
    public class ListNode : Node
    {
        public ListNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            var grid = new Grid {Width = 40};
            AddControlToNode(grid);

            Name = "List node";

            AddInputPortToNode("Items", typeof (object), true);
            AddOutputPortToNode("List", typeof (object));
        }

        public override void Calculate()
        {
            try
            {
                OutputPorts[0].Data = InputPorts[0].Data;

                TopComment.Text = "";
                TopComment.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                TopComment.Text = ex.ToString();
                TopComment.Visibility = Visibility.Visible;
                TopComment.HostNode_PropertyChanged(null, null);
            }
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