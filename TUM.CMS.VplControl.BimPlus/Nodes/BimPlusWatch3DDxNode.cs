using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using BimPlus.Explorer.Contract.Model;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using TUM.CMS.VplControl.Watch3D.Nodes;
using Color = System.Drawing.Color;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class BimPlusWatch3DDxNode : Watch3DNodeDx
    {
        public BimPlusWatch3DDxNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            // HelixViewport3D.Title = "BimPlus Viewer";
        }

        public void VisualizeBimPlusData()
        {
            // Get Geometry of n object if possible
            if (InputPorts[0].Data == null) return;
            if (InputPorts[0].Data.GetType() != typeof (List<GenericElement>)) return;

            var myModels = new List<MeshGeometryModel3D>();

            var meshBuilder = new MeshBuilder();

            foreach (var item in (List<GenericElement>) InputPorts[0].Data)
            {
                var points = item.AttributeGroups["geometry"].Attributes["threejspoints"] as IList<Point3D>;

                var vecs = new List<Vector3>();

                vecs.AddRange(
                    points.Select(
                        pt =>
                            new Vector3
                            {
                                X = Convert.ToSingle(pt.X),
                                Y = Convert.ToSingle(pt.Y),
                                Z = Convert.ToSingle(pt.Z)
                            }));

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
                        meshBuilder.AddQuad(vecs[indices[i + 1]], vecs[indices[i + 2]], vecs[indices[i + 3]],
                            vecs[indices[i + 4]]);
                        i = i + 4;
                    }
                }

                // Get the color of each representation group
                var color = Convert.ToInt64(item.AttributeGroups["geometry"].Attributes["color"]);
                var tempcolor = Color.FromArgb((int) color);

                var mod = new MeshGeometryModel3D
                {
                    Geometry = meshBuilder.ToMeshGeometry3D(),
                    // Material = PhongMaterials.Bronze
                    Material = PhongMaterials.Orange
                    /*
                        new PhongMaterial
                        {
                            // DiffuseColor = new SharpDX.Color(tempcolor.R, tempcolor.G, tempcolor.B, tempcolor.A)
                        }
                    */
                };

                myModels.Add(mod);

                // Refresh the Mesh Builder
                // meshBuilder = new MeshBuilder();
            }

            var model = new MeshGeometryModel3D
            {
                Geometry = meshBuilder.ToMeshGeometry3D(),
                Material = PhongMaterials.Orange,
                Visibility = Visibility.Visible
            };

            ViewPort3D.Items.Add(model);
            // _control.ViewPort3D.Items.Add(new AmbientLight3D() { Color = Color4.White });
            // _control.ViewPort3D.Items.Add(new DirectionalLight3D() { Color = Color4.White, Direction = new Vector3(0, 0, 1) });

            try
            {
                // _control.ViewPort3D.ZoomExtents();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}