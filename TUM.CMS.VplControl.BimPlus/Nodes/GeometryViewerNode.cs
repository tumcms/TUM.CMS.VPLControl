using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BimPlus.Explorer.Contract.Model;
using BimPlus.Sdk.Data.TenantDto;
using TUM.CMS.VplControl.BimPlus.Controls;
using TUM.CMS.VplControl.Nodes;
using SelectionChangedEventArgs = BimPlus.Explorer.Contract.Services.Selection.SelectionChangedEventArgs;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class GeometryViewerNode : Node
    {
        // DataController
        private readonly DataController _controller;
        public SelectionManager MSelectionManager;
        public GeometryViewerControl Viewer;

        public GeometryViewerNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            MSelectionManager = new SelectionManager();

            // DataContext = this;

            Viewer = new GeometryViewerControl();
            Viewer.InitializeComponent();

            // Add default selection list to receive the elements which where clicked in the control
            Viewer.DxControl.SelectionList = MSelectionManager.GetDefaultSelectionList();

            // Viewer.DxControl.SelectionList.SelectionChanged += SelectionListOnSelectionChanged;
            MSelectionManager.SelectionChanged += MSelectionManagerOnSelectionChanged;

            // Register the default highlighting list
            Viewer.DxControl.AddHighlighting(MSelectionManager.GetDefaultSelectionList(), Color.DarkBlue, 100);

            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            AddInputPortToNode("Model", typeof (object));
            AddInputPortToNode("HighlightElements", typeof (object));
            AddOutputPortToNode("SelectedElements", typeof (object));

            var cont = new UserControl
            {
                Content = Viewer,
                Margin = new Thickness(5, 20, 5, 5)
            };
            AddControlToNode(cont);

            // Prevent a MouseWheel Conflict -> Zooming ...  
            MouseWheel += OnMouseWheel;
            
            ResizeButton.Visibility = Visibility.Collapsed;
            QuestButton.Visibility = Visibility.Collapsed;
            CaptionLabel.Visibility = Visibility.Collapsed;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            mouseWheelEventArgs.Handled = true;
        }

        public override void Calculate()
        {
            // Check if Inpout is OK
            if (InputPorts[0] == null || ControlElements[0] == null) return;

            // Clear the Scene 
            Viewer.DxControl.ClearScene();

            var id = new Guid();

            // Type case 
            if (InputPorts[0].Data != null && InputPorts[0].Data.GetType() == typeof(DtoDivision))
            {
                var model = InputPorts[0].Data as DtoDivision;
                if (model != null && model.TopologyDivisionId != null)
                {
                    id = (Guid) model.TopologyDivisionId;
                }
            }

            if (InputPorts[0].Data.GetType() == typeof (Project))
            {
                var project = InputPorts[0].Data as Project;
                if (project != null) id = project.Id;
            }

            // Loading Routine
            if (id != Guid.Empty)
            {
                var t =
                    Task.Factory.StartNew(() => { Viewer.LoadGeometry(id, _controller.DataContainer.GetTeamSession()); });
            }

            // Element Visualization disabled
            List<GenericElement> genericElements;
            if (InputPorts[0].Data.GetType() == typeof (List<GenericElement>))
            {
                genericElements = InputPorts[0].Data as List<GenericElement>;
                if (genericElements != null)
                    foreach (var t in from item in genericElements
                        where item.Id != Guid.Empty
                        select
                            Task.Factory.StartNew(
                                () => { Viewer.LoadGeometry(item.Id, _controller.DataContainer.GetTeamSession()); }))
                    {
                    }
            }

            // Highlight Part
            if (InputPorts[1].Data == null) return;

            Viewer.DxControl.AddHighlighting(MSelectionManager.GetDefaultSelectionList(), Color.Red, 100);

            // Clear the Selection List
            // MSelectionManager.GetDefaultSelectionList().Clear();
            // Viewer.DxControl.SelectionList = MSelectionManager.GetDefaultSelectionList();

            var genericElementsHighlight = InputPorts[1].Data as List<BaseElement>;
            if (genericElementsHighlight == null) return;
            foreach (var item in genericElementsHighlight)
            {
                MSelectionManager.GetDefaultSelectionList().Select(item.Id, true);
            }

            Viewer.DxControl.SelectionList = MSelectionManager.GetDefaultSelectionList();
        }

        // Selection Manager Selection Changed Event
        private void MSelectionManagerOnSelectionChanged(object sender,
            SelectionChangedEventArgs selectionChangedEventArgs)
        {
            // Finally pass over all the selectedElements
            if (OutputPorts[0].ConnectedConnectors.Count != 0)
                OutputPorts[0].Data =
                    _controller.IntBase.GetElementsById(MSelectionManager.GetDefaultSelectionList().GetSelection());
        }

        public override Node Clone()
        {
            return new GeometryViewerNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}