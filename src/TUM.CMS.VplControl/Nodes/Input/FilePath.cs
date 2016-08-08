using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Input
{
    public class FilePathNode : Node
    {
        private readonly TextBlock textBlock;

        public FilePathNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddOutputPortToNode("String", typeof (string));

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(60, GridUnitType.Pixel)});
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(100, GridUnitType.Auto)});

            textBlock = new TextBlock {IsHitTestVisible = false, VerticalAlignment = VerticalAlignment.Center};

            textBlock.SetValue(ColumnProperty, 1);

            var button = new Button {Content = "Search"};
            button.Click += button_Click;
            button.Width = 50;

            grid.Children.Add(textBlock);
            grid.Children.Add(button);

            AddControlToNode(grid);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false
                //Filter = "vplXML (.vplxml)|*.vplxml"
            };


            if (openFileDialog.ShowDialog() == true)
            {
                textBlock.Text = openFileDialog.FileName;
                //MainContentGrid.Width = 250;
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