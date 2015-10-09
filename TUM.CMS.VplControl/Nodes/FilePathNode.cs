using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Orientation = System.Windows.Controls.Orientation;
using TextBox = System.Windows.Controls.TextBox;

namespace TUM.CMS.VplControl.Nodes
{
    public class FilePathNode : Node
    {
        private TextBox textBox;

        public FilePathNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddOutputPortToNode("FilePath", typeof (object));

            var stack = new StackPanel() {Orientation = Orientation.Horizontal};
            textBox = new TextBox() {MinWidth = 120, MaxWidth = 300, IsHitTestVisible = false, IsEnabled = false};
            var button = new Button() {Content = "Choose a File", HorizontalAlignment = HorizontalAlignment.Right};
            stack.Children.Add(textBox);
            stack.Children.Add(button);

            textBox.TextChanged += textBox_TextChanged;
            textBox.KeyUp += textBox_KeyUp;

            button.Click += ButtonOnClick;

            AddControlToNode(stack);

        }

        private void ButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            // Set filter for file extension and default file extension
            {
                Filter = "OBJ files (*.obj)|*.obj|STL files (*.stl)|*.stl|All files (*.*)|*.*",
                InitialDirectory = @"C:\",
                Title = "Please select a file."
            };

            // Display OpenFileDialog by calling ShowDialog method
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result != true) return;
            // Set TextBox
            textBox.Text = dlg.FileName;
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
            if (textBox == null && textBox.Text == null) return;
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