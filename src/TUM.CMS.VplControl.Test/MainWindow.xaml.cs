using System;
using System.IO;
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

            KeyDown += VplGroupControl.MainVplControl.VplControl_KeyDown;
            KeyUp += VplGroupControl.MainVplControl.VplControl_KeyUp;

            Loaded += OnLoaded;

            VplGroupControl.MainVplControl.ExternalNodeTypes.AddRange(
                Utilities.Utilities.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TUM.CMS.VplControl.Test.Nodes")
                    .ToList());

            VplGroupControl.MainVplControl.ExternalNodeTypes.AddRange(
                Utilities.Utilities.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TUM.CMS.VplControl.Watch3D.Nodes")
                    .ToList());

            VplGroupControl.MainVplControl.ExternalNodeTypes.Add(typeof (ScriptingNode));
            VplGroupControl.MainVplControl.ExternalNodeTypes.Add(typeof (Watch3DNode));

            VplGroupControl.MainVplControl.NodeTypeMode = NodeTypeModes.All;

            //VplPropertyGrid.SelectedObject = VplControl;

        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var filePath = @"../testdata/test.vplxml";
            if (File.Exists(filePath))
            {
                VplControl.OpenFile(filePath);
                VplGroupControl.MainVplControl.OpenFile(filePath);
            }
        }
    }
}