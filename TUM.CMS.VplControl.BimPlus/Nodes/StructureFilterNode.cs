using System.Collections.Generic;
using BimPlus.Explorer.Contract.Model;
using TUM.CMS.VplControl.BimPlus.Controls;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{

    internal class StructureFilterNode : Node
    {
        private readonly DataController _controller;

        private readonly StructureFilterNodeControl _control;

        public StructureFilterNode(VplControl hostCanvas)
            : base(hostCanvas)
        {
            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            _control = new StructureFilterNodeControl();

            AddControlToNode(_control);

            AddInputPortToNode("Input", typeof (object));

            AddOutputPortToNode("FilteredElements", typeof(object));

            DataContext = this;
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data == null || InputPorts[1].Data == null)
                return;

            var filters = _controller.IntBase.GetFilters();

            if (_control.FilterStringTextBox.Text != "")
            {
                // _controller.IntBase.SetFilter("NewFilter", "control.FilterStringTextBox.Text", );
            }

            var filteredElements = new List<GenericElement>();

            OutputPorts[0].Data = filteredElements;
        }

        public override Node Clone()
        {
            return new StructureFilterNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}