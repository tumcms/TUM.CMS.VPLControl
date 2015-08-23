using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BimPlus.Explorer.Contract.Model;
using HelixToolkit.Wpf;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Watch3D.Controls;
using Color = System.Drawing.Color;
using GeometryModel3D = System.Windows.Media.Media3D.GeometryModel3D;
using MeshBuilder = HelixToolkit.Wpf.MeshBuilder;
using ObjReader = HelixToolkit.Wpf.ObjReader;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace TUM.CMS.VplControl.Watch3D.Nodes
{
    public class Watch3DNode : Node
    {
        private HelixViewport3D HelixViewport3D { get; set; }

        private BackgroundWorker _worker;

        private Watch3DControl _control;

        private List<GenericElement> BuildingElements { get; set; }
        private List<MeshIdandGeometry> myGeometry { get; set; }

        private Dictionary<Guid, GeometryModel3D> _IdToModel { get; set; }
        private Dictionary<Guid, Material> _IdToMaterial { get; set; }

        private readonly Material _selectionMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Crimson));

        private IList<GeometryModel3D> selectedModels = new List<GeometryModel3D>();

        public Watch3DNode(VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddInputPortToNode("Object", typeof (object));

            _control = new Watch3DControl();
            AddControlToNode(_control);

            HelixViewport3D = _control.ViewPort3D;

            // Selection Commands
            PointSelectionCommand = new PointSelectionCommand(HelixViewport3D.Viewport, HandleSelectionEvent);
            HelixViewport3D.InputBindings.Add(new MouseBinding(PointSelectionCommand, new MouseGesture(MouseAction.LeftClick)));
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data != null && InputPorts[0].Data.GetType() == typeof(List<GenericElement>))
                BuildingElements = InputPorts[0].Data as List<GenericElement>;

            _worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            _worker.DoWork += WorkerOnDoWork;
            _worker.ProgressChanged += WorkerOnProgressChanged;
            _worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            _worker.RunWorkerAsync(10000);
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            // Implement progressBar
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                _control.progBar.Value = progressChangedEventArgs.ProgressPercentage;
            });
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (runWorkerCompletedEventArgs.Result == null)
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    _control.progBar.Visibility = Visibility.Collapsed;
                    _control.progLabel.Visibility = Visibility.Collapsed;
                });
                return;
            }
                

            Dispatcher.BeginInvoke((Action)delegate ()
            {
                _control.progBar.Visibility = Visibility.Collapsed;
                _control.progLabel.Visibility = Visibility.Collapsed;
            });

            var container = new ContainerUIElement3D();

            // BimPlusData
            if (runWorkerCompletedEventArgs.Result.GetType() == typeof (List<MeshIdandGeometry>))
            {
                var geometry = runWorkerCompletedEventArgs.Result as List<MeshIdandGeometry>;

                // Set it globally 
                myGeometry = geometry;
               

                if (myGeometry != null)
                    foreach (var mygeometry in myGeometry)
                    {
                        var newgeometry = mygeometry.Model3D.Clone();
                        var element = new ModelUIElement3D { Model = newgeometry };
                        // Geometryhandler.SetMeshIdAndGeometry(mygeometry.Id, newgeometry, mygeometry.Material);
                        element.MouseDown += (sender1, e1) => OnElementMouseDown(sender1, e1, this);
                        container.Children.Add(element);
                    }

                // First of all clear the view 
                HelixViewport3D.Children.Clear();
                HelixViewport3D.Children.Add(container);
                HelixViewport3D.CameraController.ZoomExtents();
                HelixViewport3D.Children.Add(new DefaultLights());
            }
            // OtherData
            else if (runWorkerCompletedEventArgs.Result is ModelVisual3D)
            {
                var modelGroup = runWorkerCompletedEventArgs.Result as Model3DGroup;

                /*
                if (modelGroup != null)
                    Dispatcher.BeginInvoke((Action)delegate () {
                        HelixViewport3D.Children.Clear();
                        HelixViewport3D.Children.Add(modelGroup.Children);
                        HelixViewport3D.CameraController.ZoomExtents();
                        HelixViewport3D.Children.Add(new DefaultLights());
                    });
                */
            }
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var container = new ContainerUIElement3D();

            var geometry = new List<MeshIdandGeometry>();
            var visModel = new List<MeshIdandGeometry>();

            if (InputPorts[0].Data == null)
                return;

            Dispatcher.BeginInvoke((Action)delegate ()
            {
                _control.progBar.Visibility = Visibility.Visible;
                _control.progLabel.Visibility = Visibility.Visible;
            });

            if (InputPorts[0].Data.GetType() == typeof(List<GenericElement>))
            {
                doWorkEventArgs.Result = VisualizeBimPlusData(InputPorts[0].Data as List<GenericElement>, sender);
            }
            else if (InputPorts[0].Data is string)
            {
                if(InputPorts[0].Data != null)
                    doWorkEventArgs.Result = VisualizeOtherData(InputPorts[0].Data as string, sender);
            }
        }

        public List<MeshIdandGeometry> VisualizeBimPlusData(List<GenericElement> baseElements, object sender)
        {
            if (InputPorts[0].Data == null)
                return null;

            var max = baseElements.Count;
            var m_i = 1;

            // Init some lists and containers
            var container = new ContainerUIElement3D();
            var geometry = new List<MeshIdandGeometry>();

            _IdToModel = new Dictionary<Guid, GeometryModel3D>();
            _IdToMaterial = new Dictionary<Guid, Material>();

            // Init the MeshBuilde
            var meshBuilder = new MeshBuilder(false, false);

            // Loop the items of each list
            foreach (var item in baseElements)
            {
                // Get the geometric data from the AttributeGroups
                var points = item.AttributeGroups["geometry"].Attributes["threejspoints"] as IList<Point3D>;
                var triangleindices = item.AttributeGroups["geometry"].Attributes["geometryindices"];
                var indices =
                    (from index in triangleindices as IList<uint> select Convert.ToInt32(index)).ToList();

                for (var i = 0; i < indices.Count; i++)
                {
                    if (indices[i] == 0)
                    {
                        meshBuilder.AddTriangle(points[indices[i + 1]], points[indices[i + 2]],
                            points[indices[i + 3]]);

                        i = i + 3;
                    }
                    else if (indices[i] == 1)
                    {
                        meshBuilder.AddQuad(points[indices[i + 1]], points[indices[i + 2]],
                            points[indices[i + 3]],
                            points[indices[i + 4]]);
                        i = i + 4;
                    }
                }

                // Get the color of each representation group
                var color = Convert.ToInt64(item.AttributeGroups["geometry"].Attributes["color"]);
                var tempcolor = Color.FromArgb((int)color);

                var col = new System.Windows.Media.Color
                {
                    A = tempcolor.A,
                    G = tempcolor.G,
                    R = tempcolor.R,
                    B = tempcolor.B
                };

                var myGeometryModel = new GeometryModel3D
                {
                    Material = new DiffuseMaterial(new SolidColorBrush(col)),
                    Geometry = meshBuilder.ToMesh(true),
                    Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90))
                };

                _IdToModel.Add(item.Id, myGeometryModel);
                _IdToMaterial.Add(item.Id, myGeometryModel.Material);

                myGeometryModel.Freeze();

                var meshAndId = new MeshIdandGeometry
                {
                    Id = item.Id,
                    Model3D = myGeometryModel,
                    Material = myGeometryModel.Material
                };

                geometry.Add(meshAndId);

                // Save the parsed information in the elements attribute group
                item.AttributeGroups["geometry"].Attributes["parsedGeometry"] = meshAndId;

                // Refresh the builder so that we do not duplicate the meshes 
                meshBuilder = new MeshBuilder(false, false);

                m_i++;
                var progressPercentage = Convert.ToInt32(((double)m_i / max) * 100);
                var backgroundWorker = sender as BackgroundWorker;
                backgroundWorker?.ReportProgress(progressPercentage, item.Id);
            }

            container.Children.Clear();
            return geometry;
        }

        public ModelVisual3D VisualizeOtherData(string path, object sender)
        {
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                _control.progBar.IsIndeterminate = true;
                _control.progLabel.Visibility = Visibility.Visible;
            });

            var extension = Path.GetExtension(path);
            var visModel = new ModelVisual3D();

            var flag = false;

            switch (extension)
            {
                case ".obj":
                    var currentHelixObjReader = new ObjReader();
                    try
                    {
                        var myModel = currentHelixObjReader.Read((string)InputPorts[0].Data);

                        // Create Visual with Content
                        visModel = new ModelVisual3D {Content = myModel};

                        // visModel.Children.Add(myModel);
                        flag = true;
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
                        var myModel = currentHelixStlReader.Read((string)InputPorts[0].Data);
                        // Create Visual with Content
                        visModel = new ModelVisual3D();
                        // visModel.Content = myModel;

                        flag = true;
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                    break;
            }

            return flag ? visModel : null;
        }


        private void OnElementMouseDown(object sender, MouseButtonEventArgs e, Watch3DNode watch3DNode)
        {
            // First check
            if (e == null) throw new ArgumentNullException(nameof(e));

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var element = sender as ModelUIElement3D;

            var geometryModel3D = element?.Model as GeometryModel3D;

            if (geometryModel3D == null)
                return;
            // If it is already selected ... Deselect
            if (selectedModels.Contains(geometryModel3D))
            {
                ResetMaterial();
            }
            // If not ... Select!
            else
            {
                selectedModels.Add(geometryModel3D);
                geometryModel3D.Material = _selectionMaterial;
            }
            
            e.Handled = true;
        }

        public PointSelectionCommand PointSelectionCommand { get; }


        private void HandleSelectionEvent(object sender, ModelsSelectedEventArgs args)
        {
            // selectedModels = args.selectedModels;
            // ChangeMaterial(selectedModels, MaterialHelper.CreateMaterial(Colors.Blue));
        }

        private static void ChangeMaterial(IEnumerable<Model3D> models, Material material)
        {
            if (models == null) return;

            foreach (var geometryModel in models.OfType<GeometryModel3D>())
            {
                geometryModel.Material = geometryModel.BackMaterial = material;
            }
        }

        private void ResetMaterial()
        {
            foreach (var element in BuildingElements)
            {
                var model = _IdToModel[element.Id];
                // model.SetValue(Material = _IdToMaterial[element.Id]);
            }
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