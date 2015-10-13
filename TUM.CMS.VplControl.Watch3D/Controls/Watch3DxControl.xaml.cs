using System.Windows.Controls;
using HelixToolkit.Wpf.SharpDX;

namespace TUM.CMS.VplControl.Watch3D.Controls
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Watch3DxControl
    {
        public Watch3DxControl()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(RenderTechnqiueComboBox.Items.CurrentItem is RenderTechnique)) return;

            // Detouch Renderer
            ViewPort3D.RenderTechnique = null;
            // Attach
            ViewPort3D.RenderTechnique = (RenderTechnique) RenderTechnqiueComboBox.Items.CurrentItem;
        }
    }
}