using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;
using TUM.CMS.VplControl.IFC.Utilities;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.IFC.Nodes
{
    public class IfcNode : Node
    {
        private readonly TextBox _textBox;
        private readonly IfcReader _ifcReader;

        public IfcNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddOutputPortToNode("IFCFile", typeof (object));

            var stack = new StackPanel {Orientation = Orientation.Horizontal};
            _textBox = new TextBox {MinWidth = 120, MaxWidth = 300, IsHitTestVisible = false, IsEnabled = false};
            var button = new Button {Content = "Choose a File", HorizontalAlignment = HorizontalAlignment.Right};
            stack.Children.Add(_textBox);
            stack.Children.Add(button);

            _textBox.TextChanged += textBox_TextChanged;
            _textBox.KeyUp += textBox_KeyUp;

            button.Click += ButtonOnClick;

            AddControlToNode(stack);

            // Init the Reader
            _ifcReader = new IfcReader();
        }

        private void ButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "IFC files (*.ifc)|*.ifc|All files (*.*)|*.*",
                InitialDirectory = @"C:\",
                Title = "Please select an IFC file."
            };

            // Display OpenFileDialog by calling ShowDialog method
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result != true) return;
            // Set TextBox
            _textBox.Text = dlg.FileName;
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
            if (_textBox == null && _textBox.Text == null) return;
                _ifcReader.ReadIfc(_textBox.Text);

            // OutputPorts[0].Data = _textBox.Text;
        }

        public override Node Clone()
        {
            return new IfcNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}