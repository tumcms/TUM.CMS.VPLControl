using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Math
{
    public class RoundNode : Node
    {
        public RoundNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddInputPortToNode("Value", typeof(double));
            AddInputPortToNode("Decimal", typeof(double));

            AddOutputPortToNode("RoundedValue", typeof(double));

            var label = new Label
            {
                Content = "Rnd",
                FontSize = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            AddControlToNode(label);
        }

        public override void Calculate()
        {
            OutputPorts[0].Data = System.Math.Round((Double)InputPorts[0].Data, (int)InputPorts[1].Data);
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
            return new RoundNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}