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
            AddInputPortToNode("Values", typeof(double), true);
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
            var t = InputPorts[0].Data.GetType();

            if (t.IsGenericType)
            {
                var collection = InputPorts[0].Data as ICollection;
                if (collection == null) return;

                List<double> doubles = collection.Cast<double>().ToList();

                OutputPorts[0].Data = doubles.Max();
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