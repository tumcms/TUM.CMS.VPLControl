using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Math
{
    public class AbsNode : Node
    {
        public AbsNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddInputPortToNode("Value", typeof (object));
            AddOutputPortToNode("AbsValue", typeof (object));
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data.IsNumber())
            {
                if (InputPorts[0].Data is int)
                    OutputPorts[0].Data = System.Math.Abs((int) InputPorts[0].Data);
                else if (InputPorts[0].Data is double)
                    OutputPorts[0].Data = System.Math.Abs((double) InputPorts[0].Data);
                else
                    OutputPorts[0].Data = null;
            }
            else
                OutputPorts[0].Data = null;
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