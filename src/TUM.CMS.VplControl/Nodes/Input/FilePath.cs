using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Input
{
    public class FilePathNode : Node
    {
        private TextBlock textBlock;

        public FilePathNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddOutputPortToNode("String", typeof (string));

            Grid grid=new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

             textBlock = new TextBlock(){MinWidth = 120, MaxWidth = 300, IsHitTestVisible = false};

            var button = new Button {Content = "Search"};
            button.Click += button_Click;
            button.Width = 50;

            grid.Children.Add(textBlock);
            grid.Children.Add(button);

            AddControlToNode(grid);
        }

        void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                //Filter = "vplXML (.vplxml)|*.vplxml"
            };


            if (openFileDialog.ShowDialog() == true)
            {
                textBlock.Text = openFileDialog.FileName;
                Calculate();
            }
        }

        private void TextNode_MouseLeave(object sender, MouseEventArgs e)
        {
            Border.Focusable = true;
            Border.Focus();
            Border.Focusable = false;
        }



        public override void Calculate()
        {
            OutputPorts[0].Data = textBlock.Text;
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
            return new FilePathNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}