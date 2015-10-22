using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Test.Nodes
{
    public class TemplateNode : Node
    {
        public TemplateNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddInputPortToNode("Test", typeof (object));
            AddOutputPortToNode("Test", typeof (object));

            AddControlToNode(new Label {Content = "TemplateNode"});
        }

        public override void Calculate()
        {
            OutputPorts[0].Data = InputPorts[0].Data;
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
            return new TemplateNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}