using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes
{
    public class CommentNode : Node
    {
        public CommentNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddControlToNode(new TextBox
            {
                MinWidth = 150,
                MaxWidth = 300,
                Text = "Your comment",
                TextWrapping = TextWrapping.WrapWithOverflow
            });
        }

        public override void Calculate()
        {
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var textBox = ControlElements[0] as TextBox;
            if (textBox == null) return;

            xmlWriter.WriteStartAttribute("Text");
            xmlWriter.WriteValue(textBox.Text);
            xmlWriter.WriteEndAttribute();
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            var value = xmlReader.GetAttribute("Text");

            var textBox = ControlElements[0] as TextBox;
            if (textBox == null) return;

            textBox.Text = value;
        }

        public override Node Clone()
        {
            return new CommentNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}