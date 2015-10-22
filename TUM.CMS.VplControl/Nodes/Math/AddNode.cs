using System;
using System.CodeDom;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Core;
using Xceed.Wpf.Toolkit.Panels;

namespace TUM.CMS.VplControl.Nodes.Math
{
    public class AddNode : Node
    {
        public AddNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddInputPortToNode("Value1", typeof (double));
            AddInputPortToNode("Value2", typeof(double));

            AddOutputPortToNode("Value", typeof(double));

            var label = new Label
            {
                Content = "+",
                Width = 60,
                FontSize = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            AddControlToNode(label);
        }

        public override void Calculate()
        {
            OutputPorts[0].Data = Double.Parse(InputPorts[0].Data.ToString()) + Double.Parse(InputPorts[1].Data.ToString());
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
            return new AddNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}