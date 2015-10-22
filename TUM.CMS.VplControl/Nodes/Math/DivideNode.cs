using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Math
{
    public class DivideNode : Node
    {
        public DivideNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddInputPortToNode("Value1", typeof (object));
            AddInputPortToNode("Value2", typeof (object));

            AddOutputPortToNode("Value", typeof (object));
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data.IsNumber() && InputPorts[1].Data.IsNumber())
            {
                if (InputPorts[0].Data is int)
                    OutputPorts[0].Data = (int) InputPorts[0].Data/(int) InputPorts[1].Data;
                else if (InputPorts[0].Data is double)
                    OutputPorts[0].Data = (double) InputPorts[0].Data/(double) InputPorts[1].Data;
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
            return new DivideNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}