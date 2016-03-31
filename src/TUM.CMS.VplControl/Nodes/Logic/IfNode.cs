using System.Windows;
using System.Windows.Controls;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Logic
{
    class IfNode : Node
    {
        public IfNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddInputPortToNode("Condition", typeof(bool));
            AddInputPortToNode("True value", typeof(object));
            AddInputPortToNode("False value", typeof(object));
             
            var label = new Label
            {
                Content = "If",
                Width = 60,
                FontSize = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            AddControlToNode(label);

            AddOutputPortToNode("Result", typeof (object));
        }

        public override void Calculate()
        {
            var data = InputPorts[0].Data;
            if (data != null && (bool) data)
            {
                OutputPorts[0].Data = InputPorts[1].Data;
            }
            else if (data != null)
            {
                OutputPorts[0].Data = InputPorts[2].Data;
            }
        }

        public override Node Clone()
        {
            var node = new IfNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };

            return node;
        }
    }
}
