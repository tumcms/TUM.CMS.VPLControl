using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Math
{
    public class SumNode : Node
    {
        public SumNode(Core.VplControl hostCanvas) : base(hostCanvas)

        {
            AddInputPortToNode("Values", typeof (double), true);

            AddOutputPortToNode("Value", typeof (double));

            var label = new Label
            {
                Content = "Sum",
                Width = 60,
                FontSize = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            AddControlToNode(label);
        }

        public override void Calculate()
        {
            double sum = 0;

            var collection = InputPorts[0].Data as ICollection;
            if (collection != null)
            {
                foreach (var obj in collection)
                    sum += double.Parse(obj.ToString());
            }
            OutputPorts[0].Data = sum;
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
            return new SumNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}