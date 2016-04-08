using System.Linq;
using System.Reflection;
using System.Windows;
using TUM.CMS.VplControl.Core;
using TUM.CMS.VplControl.Utilities;
using TUM.CMS.VplControl.Watch3D.Nodes;
using TUM.CMS.VPL.Scripting.Nodes;

namespace TUM.CMS.VplControl.Test
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            KeyDown += VplControl.VplControl_KeyDown;
            KeyUp += VplControl.VplControl_KeyUp;

            VplControl.ExternalNodeTypes.AddRange(
                ClassUtility.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TUM.CMS.VplControl.Test.Nodes")
                    .ToList());

            VplControl.ExternalNodeTypes.AddRange(
                ClassUtility.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TUM.CMS.VplControl.Watch3D.Nodes")
                    .ToList());

            VplControl.ExternalNodeTypes.Add(typeof (ScriptingNode));
            VplControl.ExternalNodeTypes.Add(typeof (Watch3DNode));

            VplControl.NodeTypeMode = NodeTypeModes.All;


            VplPropertyGrid.SelectedObject = VplControl;
        }
    }
}