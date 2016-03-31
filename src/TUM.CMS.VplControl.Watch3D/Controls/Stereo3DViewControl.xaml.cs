using System.Windows;

namespace TUM.CMS.VplControl.Watch3D.Controls
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Stereo3DViewControl
    {
        public Stereo3DViewControl()
        {
            InitializeComponent();
        }

        private void Stereo3DViewControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            StereoView3D.InitializeComponent();
            StereoView3D.SynchronizeStereoModel();
        }
    }
}