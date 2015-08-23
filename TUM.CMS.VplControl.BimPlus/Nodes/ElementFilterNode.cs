using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BimPlus.Explorer.Contract.Model;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class ElementFilterNode : Node
    {
        private readonly DataController _controller;

        public ElementFilterNode(VplControl hostCanvas)
            : base(hostCanvas)
        {
            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            var lbl = new Label{Content = "Filter Node ", Margin = new Thickness(5,15, 5, 10)};
            AddInputPortToNode("Input", typeof (object));
            AddInputPortToNode("FilterCriteria", typeof(object));

            AddOutputPortToNode("FilteredElements", typeof(object));

            DataContext = this;
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data == null || InputPorts[1].Data == null)
                return;

            var filteredElements = new List<GenericElement>();
            foreach (var item in (IEnumerable) InputPorts[0].Data)
            {
                var elementTypeInfo = InputPorts[1].Data as ElementTypeInfo;
                var genericElement = item as GenericElement;
                if (genericElement != null && (elementTypeInfo != null && genericElement.TypeName == elementTypeInfo.Name.Substring(3)))
                {
                    filteredElements.Add(item as GenericElement);
                }
            }
            OutputPorts[0].Data = filteredElements;
        }

        public override Node Clone()
        {
            return new ElementFilterNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}