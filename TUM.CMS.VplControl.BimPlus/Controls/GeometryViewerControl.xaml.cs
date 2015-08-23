using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BimPlus.Sdk.Data.Geometry;
using BimPlus.Sdk.Utilities.V2;
using Nemetschek.Allready.Logistics.DbCore;
using Nemetschek.DXBase.Control;
using Nemetschek.NUtilLibrary;
using Newtonsoft.Json.Linq;
using DXControl = BimPlus.Explorer.DXControl.DXControl;

namespace TUM.CMS.VplControl.BimPlus.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GeometryViewerControl
    {
        protected SelectionManager MSelectionManager;

        public DXControl DxControl;

        public GeometryViewerControl()
        {
            MSelectionManager = new SelectionManager();
            InitializeComponent();

            // Create the DXControl
            DxControl = new DXControl();
            controlGrid.Children.Add(DxControl);

            // Add default selection list to receive the elements which where clicked in the control
            DxControl.SelectionList = MSelectionManager.GetDefaultSelectionList();

            // Register the default highlighting list
            // DxControl.AddHighlighting(MSelectionManager.GetDefaultSelectionList(), System.Drawing.Color.Red, 100);
        }

        public void LoadGeometry(Guid topologyId, ITeamSession session)
        {
            var geometry = new Dictionary<Guid, List<CElementPolyeder>>();

            // Getting all geometry might take a while
            session.Timeout = System.Threading.Timeout.Infinite;

            // Get project geometry
            var models = session.GetObjectGeometry(topologyId);
            // session.Get
            foreach (var elem in models.Objects)
            {
                // Skip invisible elements like opening bodies
                if (elem.ExistProperty("Geometry", "Visible"))
                {
                    if (elem.GetBooleanProperty("Geometry", "Visible") == false)
                        continue;
                }

                // Get tje actual polyhedron data encoded as Base64 string
                var data = elem.GetStringProperty("Geometry", "Picture");
                var nulLcompress = elem.GetInt16Property("Geometry", "Compress");
                var compress = (byte)(nulLcompress ?? 1);
                if (data == null)
                    continue;

                // Decode and deserialize polyhedron
                var bytePolyeder = Convert.FromBase64String(data);
                var polyeder = DeserializeVarbinaryMax.DeserializePolyeder(bytePolyeder, compress);

                // If polyhedron consists only of edges then create tube from vertices
                if (polyeder.face.Count == 0 && polyeder.edge.Count > 0)
                    polyeder = Tubes.CreateTube(polyeder);

                if (polyeder == null || polyeder.face.Count == 0 || polyeder.edge.Count == 0 || polyeder.point.Count == 0)
                    continue;

                // Polyeder can consist of several sub-polyeders
                var groupId = Guid.Empty;
                foreach (var json in (from @group in elem.AttributeGroups where @group.Key.Equals("related_objects") select @group.Value["0"]).OfType<JContainer>())
                {
                    groupId = Guid.Parse(json.Value<string>("id"));
                }

                polyeder.ID = elem.Id;

                // Get and apply transformation matrix
                var stringMatrix = elem.GetStringProperty("Geometry", "matrix");
                if (!string.IsNullOrEmpty(stringMatrix))
                {
                    var mat = new CMatrix(Convert.FromBase64String(stringMatrix));
                    polyeder.MultiplyWithMatrix(mat.Values);
                }

                var elemPolyederId = groupId != Guid.Empty ? groupId : elem.Id;

                var elemPolyeder = new CElementPolyeder();
                elemPolyeder.AddBasePolyeder(polyeder);
                elemPolyeder.ID = elemPolyederId;

                // Get color information
                var col = elem.GetProperty("Geometry", "color");
                elemPolyeder.color = col != null ? System.Drawing.Color.FromArgb((int)Convert.ToUInt32(col)) : System.Drawing.Color.FromArgb(255, 128, 128, 128);

                // Assign polyhedron to its group
                List<CElementPolyeder> polyeders;

                if (geometry.TryGetValue(elemPolyederId, out polyeders))
                {
                    polyeders.Add(elemPolyeder);
                }
                else
                {
                    polyeders = new List<CElementPolyeder> {elemPolyeder};
                    geometry.Add(elemPolyederId, polyeders);
                }

            }

            // Convert geometries to meshes
            var meshes = geometry.Select(elem => new CDxObjectMesh(elem.Key, elem.Value)).ToList();

            // Add the meshes to the d3d control
            DxControl.AddPolyeders(meshes);
        }

        private void UIElement_OnDragLeave(object sender, DragEventArgs e)
        {
            var ptSource = e.GetPosition(DxControl);
            var ptTarget = new Point();
            if (e.KeyStates == DragDropKeyStates.LeftMouseButton)
            {
                ptTarget = e.GetPosition(DxControl);
            }

            if (ptTarget.X != 0 || ptTarget.Y != 0) return;
            DxControl.Width += ptTarget.X - ptSource.X;
            DxControl.Width += ptTarget.Y - ptSource.Y;
        }
    }
}
