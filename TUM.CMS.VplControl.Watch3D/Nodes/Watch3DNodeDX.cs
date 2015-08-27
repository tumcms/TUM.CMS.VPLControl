using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Watch3D.Controls;
using Color = System.Windows.Media.Color;
using ColorConverter = HelixToolkit.Wpf.SharpDX.ColorConverter;
using MeshBuilder = HelixToolkit.Wpf.SharpDX.MeshBuilder;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;
using ObjReader = HelixToolkit.Wpf.SharpDX.ObjReader;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;

namespace TUM.CMS.VplControl.Watch3D.Nodes
{
    public class Watch3DNodeDx : Node
    {
        public Watch3DxControl _control;

        public Watch3DNodeDx(Core.VplControl hostCanvas): base(hostCanvas)
        {
            // Add Control s
            _control = new Watch3DxControl();
            AddControlToNode(_control);

            // Set Node resizable ...
            IsResizeable = true;

            // Add Control s
            _control = new Watch3DxControl
            {
                ViewPort3D =
                {
                    Background = new SolidColorBrush(Colors.Transparent),
                    Camera =
                        new PerspectiveCamera
                        {
                            Position = new Point3D(3, 3, 5),
                            NearPlaneDistance = 0.1,
                            FarPlaneDistance = double.PositiveInfinity,
                            LookDirection = new Vector3D(-3, -3, -5),
                            UpDirection = new Vector3D(0, 1, 0)
                        },
                    RenderTechnique = Techniques.RenderBlinn,
                    MaximumFieldOfView = 45,
                    MinimumFieldOfView = 20
                }
            };

            ViewPort3D = _control.ViewPort3D;

            var mb = new MeshBuilder();
            for (var i = 0; i < 1000; i++)
            {
                mb.AddBox(new Vector3(i + 15, i + 15, i + 15), 100, 100, 100, BoxFaces.All);
            }

            // _control.meshModel.Material = PhongMaterials.Orange;
            // _control.meshModel.Geometry = mb.ToMeshGeometry3D();
            //_control.meshModel.Visibility = Visibility.Visible;
            
            var model = new MeshGeometryModel3D()
            {Geometry = mb.ToMeshGeometry3D(), Material = PhongMaterials.Orange, Visibility = Visibility.Visible};
            _control.ViewPort3D.Items.Add(model);

            _control.ViewPort3D.Items.Add(new AmbientLight3D() {Color = Color4.White});
            _control.ViewPort3D.Items.Add(new DirectionalLight3D() { Color = Color4.White , Direction = new Vector3(0,0,1)});

            // _control.ViewPort3D.ShowTriangleCountInfo = true;
            // _control.ViewPort3D.ShowFieldOfView = true;

            AddControlToNode(_control);

            AddInputPortToNode("Elements", typeof(object));
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data == null) return;

            if (InputPorts[0].Data is string)
            {
                var extension = Path.GetExtension(InputPorts[0].Data as string);

                var flag = false;

                switch (extension)
                {
                    case ".obj":
                        var currentHelixObjReader = new ObjReader();
                        try
                        {
                            var objModel = currentHelixObjReader.Read((string)InputPorts[0].Data);
                            var modelGroup = new GroupModel3D();
                            var modelGeometry = new Element3DCollection();
                            modelGeometry.AddRange(objModel.Select(x => new MeshGeometryModel3D() { Geometry = x.Geometry as MeshGeometry3D, Material = x.Material, }));

                            modelGroup.Children = modelGeometry;

                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                _control.ViewPort3D.Items.Add(modelGroup);
                            });
                          
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
                            // _control.ViewPort3D.Items.Add(myModel);
                        }
                        catch (Exception)
                        {
                            // ignore
                        }
                        break;
                }
            }
        }

        public override Node Clone()
        {
            return new Watch3DNodeDx(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}