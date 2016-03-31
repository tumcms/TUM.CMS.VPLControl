using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Math
{
    public class AbsNode : Node
    {
        public AbsNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddInputPortToNode("Value", typeof (double));
            AddOutputPortToNode("AbsValue", typeof (double));

            var label = new Label
            {
                Content = "Abs",
                Width = 60,
                FontSize = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            AddControlToNode(label);
        }

        public override void Calculate()
        {
            OutputPorts[0].Data = System.Math.Abs(Double.Parse(InputPorts[0].Data.ToString()));
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            // add your xml serialization methods here
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            // add your xml deserialization methods here
        }

        public override Node Clone()
        {
            return new AbsNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}