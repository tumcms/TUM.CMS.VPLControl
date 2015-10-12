using System.Windows;
using System.Windows.Controls;
using System.Xml;
using BimPlus.IntegrationFramework.Contract.Model;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class ElementTypeNode : Node
    {
        // DataController
        private readonly DataController _controller;
        private readonly ComboBox _typeComboBox;

        public ElementTypeNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            DataContext = this;

            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;


            AddOutputPortToNode("ElementType", typeof (object));

            _typeComboBox = new ComboBox
            {
                DisplayMemberPath = "Name",
                Margin = new Thickness(5, 20, 5, 15)
            };
            _typeComboBox.SelectionChanged += SelectionChanged;

            if (_controller.IntBase.APICore != null)
            {
                _typeComboBox.ItemsSource = _controller.IntBase.APICore.GetElementTypes();
            }
            AddControlToNode(_typeComboBox);

            QuestButton.Visibility = Visibility.Visible;
            BinButton.Visibility = Visibility.Visible;

            // TopComment.Visibility = Visibility.Visible;
            // BottomComment.Visibility = Visibility.Visible;
        }

        public override void Calculate()
        {
            // Output Part
            if (_typeComboBox != null) OutputPorts[0].Data = _typeComboBox.SelectedItem as ElementTypeInfo;
        }

        public override Node Clone()
        {
            return new ElementTypeNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var controlElement = ControlElements[0] as ComboBox;
            if (controlElement == null) return;

            xmlWriter.WriteStartAttribute("TypeName");
            xmlWriter.WriteValue(_typeComboBox.Name);
            xmlWriter.WriteEndAttribute();
        }

        public void SelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            Calculate();
            // if (_typeComboBox != null) OutputPorts[0].Data = _typeComboBox.SelectedItem as DtoDivision;
        }
    }
}