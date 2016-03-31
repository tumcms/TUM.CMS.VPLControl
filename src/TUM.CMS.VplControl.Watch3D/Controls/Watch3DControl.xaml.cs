using System.Windows.Media.Media3D;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;

namespace TUM.CMS.VplControl.Watch3D.Controls
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Watch3DControl
    {
        public Watch3DControl()
        {
            InitializeComponent();
        }

        private void ShowGeometry(MeshGeometry3D geometry)
        {
            var container = new ContainerUIElement3D();

            // var geometry = e.Result as List<Geometryhandler.MeshIdandGeometry>;

            var viewport = new ModelVisual3D();
            ViewPort3D.Children.Add(viewport);
            ViewPort3D.Children.Add(container);
            ViewPort3D.ShowFrameRate = true;
            ViewPort3D.ZoomExtents();
        }
    }
}