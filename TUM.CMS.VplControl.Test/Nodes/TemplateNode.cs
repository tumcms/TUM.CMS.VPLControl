using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Nodes;

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
            try
            {
                OutputPorts[0].Data = InputPorts[0].Data;

                TopComment.Text = "";
                TopComment.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                TopComment.Text = ex.ToString();
                TopComment.Visibility = Visibility.Visible;
                TopComment.HostNode_PropertyChanged(null, null);
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
            return new TemplateNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}