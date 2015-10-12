using System.Windows.Controls;

namespace TUM.CMS.VplControl.Nodes
{
    public class ListNode : Node
    {
        public ListNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            var grid = new Grid {Width = 40};
            AddControlToNode(grid);

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