using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BimPlus.IntegrationFramework.Contract.Model;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class ElementMergeNode : Node
    {
        private readonly DataController _controller;

        public ElementMergeNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            var lbl = new Label{Content = "Filter Node ", Margin = new Thickness(5,15, 5, 10)};
            AddInputPortToNode("Input", typeof (object));
            AddInputPortToNode("Input", typeof(object));

            AddOutputPortToNode("MergedElements", typeof(object));

            // LabelCaption.Visibility = Visibility.Visible;
            // LabelCaption.Content = "";
            DataContext = this;
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data == null || InputPorts[1].Data == null)
                return;

            var mergedGenericElements = new List<GenericElement>();

            if (InputPorts[0].Data.GetType() == typeof (List<GenericElement>) &&
                InputPorts[1].Data.GetType() == typeof (List<GenericElement>))
            {
                mergedGenericElements.AddRange(InputPorts[0].Data as List<GenericElement>);
                mergedGenericElements.AddRange(InputPorts[1].Data as List<GenericElement>);
            }

            OutputPorts[0].Data = mergedGenericElements;
        }

        public override Node Clone()
        {
            return new ElementMergeNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}