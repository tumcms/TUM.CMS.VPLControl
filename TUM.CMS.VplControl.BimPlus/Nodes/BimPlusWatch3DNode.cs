
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BimPlus.Explorer.Contract.Model;
using HelixToolkit.Wpf;
using TUM.CMS.VplControl.Watch3D;
using TUM.CMS.VplControl.Watch3D.Nodes;
using Color = System.Drawing.Color;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class BimPlusWatch3DNode : Watch3DNode
    {
        List<GenericElement> BuildingElements { get; set; }

        // BackgroundWorker
        public BackgroundWorker _worker;

        // Geometry Members
        protected List<MeshIdandGeometry> MyGeometry { get; set; }

        public BimPlusWatch3DNode(Core.VplControl hostCanvas): base(hostCanvas)
        {
            HelixViewport3D.Title = "BimPlus Viewer";

            // Refresh the selected Models
            SelectedModels = new List<GeometryModel3D>();
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data != null && InputPorts[0].Data.GetType() == typeof (List<GenericElement>))
            {
                BuildingElements = InputPorts[0].Data as List<GenericElement>;
                _worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
                _worker.DoWork += WorkerOnDoWork;
                _worker.ProgressChanged += WorkerOnProgressChanged;
                _worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
                _worker.RunWorkerAsync(10000);
            }
            else
            {   
                base.Calculate();
            }
               
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            // Implement progressBar
            Dispatcher.BeginInvoke((Action)delegate ()
            {
                ProgressBar.Value = progressChangedEventArgs.ProgressPercentage;
            });
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (runWorkerCompletedEventArgs.Result == null)
            {
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    ProgressLabel.Visibility = Visibility.Collapsed;
                });
                return;
            }

            Dispatcher.BeginInvoke((Action)delegate ()
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                ProgressLabel.Visibility = Visibility.Collapsed;
            });

            var container = new ContainerUIElement3D();

            // BimPlusData
            if (runWorkerCompletedEventArgs.Result.GetType() == typeof(List<MeshIdandGeometry>))
            {
                var geometry = runWorkerCompletedEventArgs.Result as List<MeshIdandGeometry>;

                // Set it globally 
                MyGeometry = geometry;

                if (MyGeometry != null)
                    foreach (var mygeometry in MyGeometry)
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
                HelixViewport3D.Children.Add(new SunLight());
                // HelixViewport3D.Children.Add(new DefaultLights());
            }
            // Other File Data
            else if (runWorkerCompletedEventArgs.Result is ModelVisual3D)
            {
                var model = runWorkerCompletedEventArgs.Result as ModelVisual3D;

                HelixViewport3D.Children.Clear();
                HelixViewport3D.Children.Add(model);
                HelixViewport3D.CameraController.ZoomExtents();
                HelixViewport3D.Children.Add(new DefaultLights());
            }
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            if (InputPorts[0].Data == null)
                return;

            Dispatcher.BeginInvoke((Action)delegate ()
            {
                ProgressBar.Visibility = Visibility.Visible;
                ProgressLabel.Visibility = Visibility.Visible;
            });

            if (InputPorts[0].Data.GetType() == typeof(List<GenericElement>))
            {
                doWorkEventArgs.Result = VisualizeBimPlusData(InputPorts[0].Data as List<GenericElement>, sender);
            }
            else if (InputPorts[0].Data is string)
            {
                if (InputPorts[0].Data != null)
                    doWorkEventArgs.Result = ReadFileData((string) InputPorts[0].Data);
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
                    BackMaterial = new DiffuseMaterial(new SolidColorBrush(col)),
                    Geometry = meshBuilder.ToMesh(true),
                    Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90))
                };

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

    }
}