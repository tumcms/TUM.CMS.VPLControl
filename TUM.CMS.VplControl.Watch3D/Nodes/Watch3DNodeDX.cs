using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using BimPlus.Explorer.Contract.Model;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Watch3D.Controls;
using GeometryModel3D = HelixToolkit.Wpf.SharpDX.GeometryModel3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;
using MeshBuilder = HelixToolkit.Wpf.SharpDX.MeshBuilder;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = BimPlus.Explorer.Contract.Model.Vector3D;

namespace TUM.CMS.VplControl.Watch3D.Nodes
{
    public class Watch3DNodeDx : Node
    {
        private Watch3DxControl _control;

        public Watch3DNodeDx(Core.VplControl hostCanvas): base(hostCanvas)
        {
            // Add Control s
            _control = new Watch3DxControl();
            AddControlToNode(_control);

            _control.ViewPort3D.ShowFrameRate = true;
            _control.ViewPort3D.ZoomExtents();

            _control.AmbientLight3D = new AmbientLight3D()
            {
                Color =  new Color4(0.1f, 0.1f, 0.1f, 1.0f)
            };

            AddInputPortToNode("Object", typeof (object));
        }

        public override void Calculate()
        {
            Visualize();
        }

        public void Visualize()
        {
            // Get Geometry of n object if possible
            if (InputPorts[0].Data.GetType() != typeof (List<GenericElement>)) return;

            var myModels = new List<MeshGeometryModel3D>();
            var mesh = new MeshGeometryModel3D();

            var meshBuilder = new MeshBuilder(false, false);

            foreach (var item in (List<GenericElement>) InputPorts[0].Data)
            {
                var points = item.AttributeGroups["geometry"].Attributes["threejspoints"] as IList<Point3D>;

                var vecs = new List<Vector3>();

                vecs.AddRange(points.Select(pt => new Vector3() { X = Convert.ToSingle(pt.X), Y = Convert.ToSingle(pt.Y), Z = Convert.ToSingle(pt.Z) }));

                var triangleindices = item.AttributeGroups["geometry"].Attributes["geometryindices"];

                var indices = (from index in triangleindices as IList<uint> select Convert.ToInt32(index)).ToList();

                for (var i = 0; i < indices.Count; i++)
                {
                    if (indices[i] == 0)
                    {
                        meshBuilder.AddTriangle(vecs[indices[i + 1]], vecs[indices[i + 2]], vecs[indices[i + 3]]);
                        i = i + 3;
                    }
                    else if (indices[i] == 1)
                    {
                        meshBuilder.AddQuad(vecs[indices[i + 1]], vecs[indices[i + 2]], vecs[indices[i + 3]], vecs[indices[i + 4]]);
                        i = i + 4;
                    }
                }

                // Get the color of each representation group
                var color = Convert.ToInt64(item.AttributeGroups["geometry"].Attributes["color"]);
                var tempcolor = System.Drawing.Color.FromArgb((int)color);

                var mod = new MeshGeometryModel3D
                {
                    Geometry = meshBuilder.ToMeshGeometry3D(),
                    // Material = PhongMaterials.Bronze
                    Material = new PhongMaterial() { DiffuseColor = new Color(tempcolor.R, tempcolor.G, tempcolor.B, tempcolor.A) }
                };

                myModels.Add(mod);

                // Refresh the Mesh Builder
                meshBuilder = new MeshBuilder();
            }

            // var meshGeometry = meshBuilder.ToMeshGeometry3D();

            var comp = new GroupModel3D();

            foreach (var item in myModels)
            {
                comp.Children.Add(item);
                // _control.meshModel.Geometry = item.Geometry;
                // _control.ViewPort3D = new Viewport3DX()
            }
                // Set the Geometry Model
            _control.groupModel = comp;
            

            // _control.groupModel.Render(new RenderContext(_control.ViewPort3D, ));
            // _control.groupModel.Render(null, null);
            // _control.meshModel.Geometry = comp.R;

            // _control.ViewPort3D.SetValue();
            //_control.ViewPort3D.

            // _control.model1.Material = PhongMaterials.Red;

            _control.ViewPort3D.ShowTriangleCountInfo = true;
            _control.ViewPort3D.ZoomExtents();
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