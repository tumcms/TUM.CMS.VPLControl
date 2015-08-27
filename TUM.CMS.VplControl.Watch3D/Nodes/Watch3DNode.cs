using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Watch3D.Controls;

namespace TUM.CMS.VplControl.Watch3D.Nodes
{
    public class Watch3DNode : Node
    {
        // Standard Selection Material
        private readonly Material _selectionMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Crimson));
        // Currently Selected Models
        public IList<GeometryModel3D> SelectedModels;
        // Control Members
        // Must be public since other objects access these control objects
        public Watch3DControl Control;
        public ProgressBar ProgressBar;
        public Label ProgressLabel;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="hostCanvas"></param>
        public Watch3DNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            // Input 
            AddInputPortToNode("Object", typeof (object));

            // Output
            AddOutputPortToNode("GeometryContainer", typeof (object));

            // Node is resizable
            IsResizeable = true;

            // Add Control
            Control = new Watch3DControl();

            // Set Control Members -> acessible for other controls instances 
            ProgressBar = Control.progBar;
            ProgressLabel = Control.progLabel;

            AddControlToNode(Control);

            // Init Viewport
            HelixViewport3D = Control.ViewPort3D;
            HelixViewport3D.Title = "Watch3D";

            // Refresh the selected Models
            SelectedModels = new List<GeometryModel3D>();
        }

        // Viewport
        protected HelixViewport3D HelixViewport3D { get; set; }

        /// <summary>
        ///     Calculation Function
        /// </summary>
        public override void Calculate()
        {
            // Clear the viewport
            HelixViewport3D.Children.Clear();

            // Check the input 
            // -> string = filepath
            // -> list of strings = multiple filepaths
            // -> ContainerUiElements = Geometric Elements
            if (InputPorts[0].Data is string)
            {
                var model = ReadFileData((string) InputPorts[0].Data);
                if (model == null) return;

                HelixViewport3D.Children.Add(model);
            }
            else if (InputPorts[0].Data.GetType() == typeof (List<object>))
            {
                foreach (var str in InputPorts[0].Data as List<object>)
                {
                    var model = ReadFileData((string) str);

                    if (model != null)
                        HelixViewport3D.Children.Add(model);
                }
            }
            else if (InputPorts[0].Data is ContainerUIElement3D)
            {
                HelixViewport3D.Children.Add(InputPorts[0].Data as ContainerUIElement3D);
            }

            // Zoom 
            HelixViewport3D.CameraController.ZoomExtents();
            // Set Standard Light
            HelixViewport3D.Children.Add(new DefaultLights());
        }

        /// <summary>
        ///     Read FileData
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ContainerUIElement3D ReadFileData(string path)
        {
            var extension = Path.GetExtension(path);
            // var visModel = new ModelVisual3D();
            var container = new ContainerUIElement3D();

            switch (extension)
            {
                case ".obj":
                    var currentHelixObjReader = new ObjReader();
                    try
                    {
                        var myModel = currentHelixObjReader.Read(path);

                        foreach (var model in myModel.Children)
                        {
                            if (model is GeometryModel3D)
                            {
                                var geom = model as GeometryModel3D;

                                var element = new ModelUIElement3D {Model = geom};
                                element.MouseDown += (sender1, e1) => OnElementMouseDown(sender1, e1, this);
                                container.Children.Add(element);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                    break;
                case ".stl":
                    var currentHelixStlReader = new StLReader();
                    try
                    {
                        var myModel = currentHelixStlReader.Read(path);

                        foreach (var model in myModel.Children)
                        {
                            if (model is GeometryModel3D)
                            {
                                var geom = model as GeometryModel3D;

                                var element = new ModelUIElement3D {Model = geom};
                                element.MouseDown += (sender1, e1) => OnElementMouseDown(sender1, e1, this);
                                container.Children.Add(element);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                    break;
            }

            return container;
        }

        /// <summary>
        ///     OnElementMouseDown: Click Event for each geometric element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="watch3DNode"></param>
        protected void OnElementMouseDown(object sender, MouseButtonEventArgs e, Watch3DNode watch3DNode)
        {
            // Check null expression
            if (e == null) throw new ArgumentNullException(nameof(e));

            // 1-CLick event
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            // Get sender
            var element = sender as ModelUIElement3D;

            // Check Type
            var geometryModel3D = element?.Model as GeometryModel3D;
            if (geometryModel3D == null)
                return;

            // If it is already selected ... Deselect
            if (SelectedModels.Contains(geometryModel3D))
            {
                geometryModel3D.Material = geometryModel3D.BackMaterial;
                SelectedModels.Remove(geometryModel3D);
            }
            // If not ... Select!
            else
            {
                SelectedModels.Add(geometryModel3D);
                geometryModel3D.Material = _selectionMaterial;
            }

            // Set selected models to Output ...  
            if (SelectedModels != null && SelectedModels.Count != 0)
            {
                var container = new ContainerUIElement3D();
                foreach (var geom in SelectedModels)
                {
                    container.Children.Add(new ModelUIElement3D {Model = geom});
                }

                OutputPorts[0].Data = container;
            }

            e.Handled = true;
        }

        public override Node Clone()
        {
            return new Watch3DNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}