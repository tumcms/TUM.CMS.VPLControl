using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TUM.CMS.VplControl.Core
{
    /// <summary>
    ///     Interaktionslogik für VplGroupControl.xaml
    /// </summary>
    public partial class VplGroupControl : UserControl
    {
        public VplGroupControl()
        {
            InitializeComponent();

            InputControl.MouseUp += InputControl_MouseUp;
            InputControl.MouseDown += InputControlOnMouseDown;

            OutputControl.MouseUp += OutputControl_MouseUp;
            OutputControl.MouseDown += OutputControlOnMouseDown;
        }


        public VplControl MainVplControl => VplControl;

        private void InputControlOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            DeselectAllNodes();
        }

        private void OutputControlOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            DeselectAllNodes();
        }

        private void DeselectAllNodes()
        {
            foreach (var selectedNode in VplControl.SelectedNodes)
            {
                selectedNode.IsSelected = false;
            }

            VplControl.SelectedNodes.Clear();
        }

        private void OutputControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MoveNodeContentFromNodeToPanel("right");
        }

        private void InputControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MoveNodeContentFromNodeToPanel("left");
        }

        private void MoveNodeContentFromNodeToPanel(string side)
        {
            foreach (var node in VplControl.SelectedNodes)
            {
                if ((node.InputPorts.Count == 0 && side == "left") || (node.OutputPorts.Count == 0 && side == "right"))
                {
                    var nodeContent = node.ContentGrid;
                    node.Children.Remove(nodeContent);

                    if (side == "left")
                    {
                        InputControl.Children.Add(nodeContent);

                        foreach (var port in node.OutputPorts)
                        {
                            node.SizeChanged -= port.ParentNode_SizeChanged;
                            node.PropertyChanged -= port.ParentNode_PropertyChanged;

                            InputControl.SizeChanged += port.ParentNode_SizeChanged;

                            port.CalcOrigin();
                        }
                    }
                    else if (side == "right")
                    {
                        OutputControl.Children.Add(nodeContent);

                        foreach (var port in node.InputPorts)
                        {
                            node.SizeChanged -= port.ParentNode_SizeChanged;
                            node.PropertyChanged -= port.ParentNode_PropertyChanged;

                            OutputControl.SizeChanged += port.ParentNode_SizeChanged;

                            port.CalcOrigin();
                        }
                    }

                    node.Delete(false);
                }
            }

            DeselectAllNodes();
        }


        private void StartPortOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var startPort = sender as Port;
            if (startPort != null)
            {
                startPort.Origin.X =
                    startPort.TranslatePoint(new Point(startPort.Width/2, startPort.Height/2), VplControl).X;
                startPort.Origin.Y =
                    startPort.TranslatePoint(new Point(startPort.Width/2, startPort.Height/2), VplControl).Y;
            }
        }

        private void EndPortOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var endPort = sender as Port;
            if (endPort != null)
            {
                endPort.Origin.X = endPort.TranslatePoint(new Point(endPort.Width/2, endPort.Height/2), VplControl).X;
                endPort.Origin.Y = endPort.TranslatePoint(new Point(endPort.Width/2, endPort.Height/2), VplControl).Y;
            }
        }
    }
}