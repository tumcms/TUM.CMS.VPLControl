using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Input
{
    public class TextNode : Node
    {
        public TextNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddOutputPortToNode("String", typeof (string));

            var textBox = new TextBox {MinWidth = 120, MaxWidth = 300, IsHitTestVisible = false};

            textBox.TextChanged += textBox_TextChanged;
            textBox.KeyUp += textBox_KeyUp;

            AddControlToNode(textBox);


            MouseEnter += TextNode_MouseEnter;
            MouseLeave += TextNode_MouseLeave;
        }

        private void TextNode_MouseLeave(object sender, MouseEventArgs e)
        {
            Border.Focusable = true;
            Border.Focus();
            Border.Focusable = false;
        }

        private void TextNode_MouseEnter(object sender, MouseEventArgs e)
        {
            ControlElements[0].Focus();
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Calculate();
        }

        public override void Calculate()
        {
            var textBox = ControlElements[0] as TextBox;
            if (textBox == null) return;

            OutputPorts[0].Data = textBox.Text;
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

            var textBox = ControlElements[0] as TextBox;
            if (textBox == null) return;

            textBox.Text = xmlReader.GetAttribute("Text");
        }

        public override Node Clone()
        {
            return new TextNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}