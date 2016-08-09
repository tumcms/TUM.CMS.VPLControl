using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Math
{
    public class MaxNode : Node
    {
        public MaxNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddInputPortToNode("Values", typeof(object), true);
            AddOutputPortToNode("MaxValue", typeof(double));

            var label = new Label
            {
                Content = "Max",
                FontSize = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            AddControlToNode(label);
        }

        public override void Calculate()
        {
            List<double> doubles = new List<double>();
            var collection = InputPorts[0].Data as ICollection;

            if (collection != null)
            {
                doubles.AddRange(from object obj in collection select double.Parse(obj.ToString()));
                OutputPorts[0].Data = doubles.Max();
            }
            else
            {
                OutputPorts[0].Data = double.Parse(InputPorts[0].Data.ToString());
            }
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
            return new MaxNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}