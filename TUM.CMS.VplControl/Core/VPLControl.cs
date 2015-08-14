using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.Win32;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TUM.CMS.VplControl.Core
{
    /// <summary>
    ///     Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///     Step 1a) Using this custom control in a XAML file that exists in the current project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:TUM.CMS.VplControl"
    ///     Step 1b) Using this custom control in a XAML file that exists in a different project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:TUM.CMS.VplControl;assembly=TUM.CMS.VplControl"
    ///     You will also need to add a project reference from the project where the XAML file lives
    ///     to this project and Rebuild to avoid compilation errors:
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///     Step 2)
    ///     Go ahead and use your control in the XAML file.
    ///     <MyNamespace:CustomControl1 />
    /// </summary>
    public class VplControl : Canvas
    {
        public MouseModes MouseMode = MouseModes.Nothing;
        private ScaleTransform scaleTransformZooming;
        private Border selectionRectangle;
        private Point startSelectionPoint;
        private TrulyObservableCollection<Node> tempCollection;
        public Line TempLine;
        internal Port TempStartPort;

        public VplControl()
        {
            try
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof (VplControl),
                    new FrameworkPropertyMetadata(typeof (VplControl)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Style = FindResource("VplControlStyle") as Style;

            MouseDown += HandleMouseDown;
            MouseMove += HandleMouseMove;
            MouseUp += HandleMouseUp;
            MouseWheel += HandleMouseWheel;
            KeyDown += VplControl_KeyDown;
            KeyUp += VplControl_KeyDown;

            NodeCollection = new TrulyObservableCollection<Node>();
            NodeGroupCollection = new List<NodeGroup>();
            ConnectorCollection = new List<Connector>();
            SelectedNodes = new TrulyObservableCollection<Node>();
            ExternalNodeTypes = new List<Type>();

            TypeSensitive = true;

            // Zooming 
            scaleTransform = new ScaleTransform();
            translateTransform = new TranslateTransform();
            transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);

            // Zooming Transformation
            LayoutTransform = transformGroup;

            // Init Grid for the background
            var visualBrush = new VisualBrush()
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 50, 50),
                ViewportUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(0, 0, 50, 50),
                ViewboxUnits = BrushMappingMode.Absolute,
                Visual = new Rectangle
                {
                    Stroke = Brushes.DarkGray,
                    StrokeThickness = 0.1,
                    Height = 1000,
                    Width = 1000,
                    Fill = Brushes.White
                }
            };

            Theme = new Theme(this) {FontFamily = TextElement.GetFontFamily(this)};

            var solidColorBrush = TextElement.GetForeground(this) as SolidColorBrush;
            if (solidColorBrush != null)
                Theme.ForegroundColor = solidColorBrush.Color;

            var colorBrush = Application.Current.Resources["BackgroundBrush"] as SolidColorBrush;
            if (colorBrush != null)
                Theme.BackgroundColor = colorBrush.Color;

            var gridBrush = Application.Current.Resources["GridBrush"] as SolidColorBrush;
            if (gridBrush != null)
                Theme.GridColor = gridBrush.Color;

            var connectorBrush = Application.Current.Resources["ConnectorBrush"] as SolidColorBrush;
            if (connectorBrush != null)
                Theme.ConnectorColor = connectorBrush.Color;

            Theme.ConnectorThickness =
                Application.Current.Resources["ConnectorThickness"] is double
                    ? (double) Application.Current.Resources["ConnectorThickness"]
                    : 0;


            var toolTipBackgroundBrush = Application.Current.Resources["TooltipBackgroundBrush"] as SolidColorBrush;
            if (toolTipBackgroundBrush != null)
            {
                Theme.TooltipBackgroundColor =
                    toolTipBackgroundBrush.Color;
            }

            var tooltipBorderBrush = Application.Current.Resources["TooltipBorderBrush"] as SolidColorBrush;
            if (tooltipBorderBrush != null)
            {
                Theme.TooltipBorderColor = tooltipBorderBrush.Color;
            }

            var portFillBrush = Application.Current.Resources["PortFillBrush"] as SolidColorBrush;
            if (portFillBrush != null)
                Theme.PortFillColor = portFillBrush.Color;

            var portStrokeBrush = Application.Current.Resources["PortStrokeBrush"] as SolidColorBrush;
            if (portStrokeBrush != null)
                Theme.PortStrokeColor = portStrokeBrush.Color;

            Theme.PortSize =
                Application.Current.Resources["PortSize"] is double
                    ? (double) Application.Current.Resources["PortSize"]
                    : 0;

            Theme.PortStrokeThickness =
                Application.Current.Resources["PortStrokeThickness"] is double
                    ? (double) Application.Current.Resources["PortStrokeThickness"]
                    : 0;

            Theme.NodeBackgroundColor = Colors.White;

            Theme.ButtonBorderColor = (Application.Current.Resources["ButtonBorderBrush"] as SolidColorBrush).Color;

            Theme.ButtonFillColor = (Application.Current.Resources["ButtonFillBrush"] as SolidColorBrush).Color;

            Theme.HighlightingColor = (Application.Current.Resources["BrushHighlighting"] as SolidColorBrush).Color;

            Theme.NodeBorderColor = (Application.Current.Resources["NodeBorderBrush"] as SolidColorBrush).Color;

            //NodeBorderThickness = Application.Current.Resources["NodeBorderThickness"] is Thickness ? (Thickness) Application.Current.Resources["NodeBorderThickness"] : new Thickness(),

            Theme.NodeBorderCornerRadius =
                Application.Current.Resources["NodeBorderCornerRadius"] is double
                    ? (double) Application.Current.Resources["NodeBorderCornerRadius"]
                    : 0;

            Theme.NodeBorderColorOnMouseOver =
                (Application.Current.Resources["NodeBorderBrushMouseOver"] as SolidColorBrush).Color;

            Theme.NodeBorderColorOnSelection =
                (Application.Current.Resources["NodeBorderBrushSelection"] as SolidColorBrush).Color;

            Theme.LineColor = (Application.Current.Resources["LineStrokeBrush"] as SolidColorBrush).Color;

            Theme.LineThickness =
                Application.Current.Resources["LineStrokeThickness"] is double
                    ? (double) Application.Current.Resources["LineStrokeThickness"]
                    : 0;

            Theme.ConnEllipseFillColor =
                (Application.Current.Resources["ConnEllipseFillBrush"] as SolidColorBrush).Color;

            Theme.ConnEllipseSize =
                Application.Current.Resources["ConnEllipseSize"] is double
                    ? (double) Application.Current.Resources["ConnEllipseSize"]
                    : 0;

        }

        internal SplineModes SplineMode { get; set; }

        [Browsable(false)]
        public TrulyObservableCollection<Node> NodeCollection { get; set; }

        [Browsable(false)]
        public List<Connector> ConnectorCollection { get; set; }

        [Browsable(false)]
        public List<NodeGroup> NodeGroupCollection { get; set; }

        [Browsable(false)]
        public TrulyObservableCollection<Node> SelectedNodes { get; set; }

        [Browsable(false)]
        public List<Type> ExternalNodeTypes { get; set; }


        [Browsable(false)]
        public NodeTypeModes NodeTypeMode { get; set; }

        // Zoom Utilities
        private readonly ScaleTransform scaleTransform;
        private readonly TransformGroup transformGroup;
        private readonly TranslateTransform translateTransform;
        public int ZoomIn { get; set; }
        public int ZoomOut { get; set; }

        [Category("All VPL settings")]
        [DisplayName(@"Type sensitive")]
        public bool TypeSensitive { get; set; }

        [Category("All VPL settings")]
        [DisplayName(@"Graph flow firection")]
        public GraphFlowDirections GraphFlowDirection { get; set; }

        [Category("All VPL settings")]
        [DisplayName(@"Graph flow firection")]
        [ExpandableObject]
        public Theme Theme { get; set; }

        private void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (MouseMode)
            {
                case MouseModes.Nothing:

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (e.ClickCount == 2)
                        {
                            // double click in empty space of canvas
                            var node = new SelectionNode(this)
                            {
                                Left = Mouse.GetPosition(this).X,
                                Top = Mouse.GetPosition(this).Y
                            };
                        }
                        else
                        {
                            var mouseUpOnNode = false;

                            foreach (var node in NodeCollection)
                            {
                                if (VisualTreeHelper.HitTest(node.Border, e.GetPosition(node)) != null)
                                    mouseUpOnNode = true;
                            }

                            // if mouse up in empty space
                            if (!mouseUpOnNode)
                            {
                                startSelectionPoint = Mouse.GetPosition(this);
                                MouseMode = MouseModes.Selection;
                                SplineMode = SplineModes.Nothing;
                            }
                        }
                    }
                    else if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        // panning
                        foreach (var node in NodeCollection)
                        {
                            node.OldMousePosition = e.GetPosition(this);

                            if (MouseMode != MouseModes.Panning)
                                MouseMove += node.HostCanvas_MouseMove;
                        }

                        MouseMode = MouseModes.Panning;
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    {
                    }

                    break;
            }
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (MouseMode == MouseModes.Selection)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (selectionRectangle == null)
                    {
                        selectionRectangle = new Border
                        {
                            // Move to WPF style
                            Background = Brushes.Transparent,
                            BorderBrush = new SolidColorBrush(Application.Current.Resources["ColorBlue"] is Color ? (Color) Application.Current.Resources["ColorBlue"] : new Color()),
                            CornerRadius = new CornerRadius(5),
                            BorderThickness = new Thickness(2)
                        };

                        SetLeft(selectionRectangle, startSelectionPoint.X);
                        SetTop(selectionRectangle, startSelectionPoint.Y);

                        Children.Add(selectionRectangle);
                    }

                    var currentPosition = Mouse.GetPosition(this);
                    var delta = Point.Subtract(currentPosition, startSelectionPoint);

                    if (delta.X < 0)
                        SetLeft(selectionRectangle, currentPosition.X);

                    if (delta.Y < 0)
                        SetTop(selectionRectangle, currentPosition.Y);

                    selectionRectangle.Width = Math.Abs(delta.X);
                    selectionRectangle.Height = Math.Abs(delta.Y);

                    foreach (var node in NodeCollection)
                    {
                        SelectedNodes.Remove(node);

                        if ((node.Left >= GetLeft(selectionRectangle) &&
                             node.Left + node.ActualWidth <= GetLeft(selectionRectangle) + selectionRectangle.Width)
                            &&
                            (node.Top >= GetTop(selectionRectangle) &&
                             node.Top + node.ActualHeight <= GetTop(selectionRectangle) + selectionRectangle.Height))
                        {
                            node.IsSelected = true;
                            SelectedNodes.Add(node);
                        }
                        else
                            node.IsSelected = false;
                    }
                }
            }


            switch (SplineMode)
            {
                case SplineModes.Nothing:
                    ClearTempLine();
                    break;
                case SplineModes.First:
                    break;
                case SplineModes.Second:
                    if (TempLine == null)
                    {
                        TempLine = new Line {Style = FindResource("VplLineStyle") as Style};
                        Children.Add(TempLine);
                    }

                    TempLine.X1 = TempStartPort.Origin.X;
                    TempLine.Y1 = TempStartPort.Origin.Y;
                    TempLine.X2 = Mouse.GetPosition(this).X;
                    TempLine.Y2 = Mouse.GetPosition(this).Y;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ClearTempLine()
        {
            Children.Remove(TempLine);
            TempLine = null;
        }

        private void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Zooming
            double scaledCanvasMouseOffsetX;
            double scaledCanvasMouseOffsetY;

            var mouseRelativetoCanvas = e.GetPosition(this);

            if (e.Delta > 0)
            {
                if (ZoomIn < 10)
                {
                    ZoomIn = ZoomIn + 1;
                    ZoomOut = ZoomOut - 1;
                    scaleTransform.ScaleX += 0.1;
                    scaleTransform.ScaleY += 0.1;

                    scaledCanvasMouseOffsetX = mouseRelativetoCanvas.X * scaleTransform.ScaleX;
                    scaledCanvasMouseOffsetY = mouseRelativetoCanvas.Y * scaleTransform.ScaleY;

                    translateTransform.X = -(scaledCanvasMouseOffsetX - mouseRelativetoCanvas.X);
                    translateTransform.Y = -(scaledCanvasMouseOffsetY - mouseRelativetoCanvas.Y);
                }
            }

            if (e.Delta < 0)
            {
                if (ZoomOut < 7)
                {
                    ZoomIn = ZoomIn - 1;
                    ZoomOut = ZoomOut + 1;
                    scaleTransform.ScaleX -= 0.1;
                    scaleTransform.ScaleY -= 0.1;

                    scaledCanvasMouseOffsetX = mouseRelativetoCanvas.X * scaleTransform.ScaleX;
                    scaledCanvasMouseOffsetY = mouseRelativetoCanvas.Y * scaleTransform.ScaleY;

                    translateTransform.X = -(scaledCanvasMouseOffsetX - mouseRelativetoCanvas.X);
                    translateTransform.Y = -(scaledCanvasMouseOffsetY - mouseRelativetoCanvas.Y);
                }
            }
        }

        private void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (MouseMode)
            {
                case MouseModes.Nothing:

                    var mouseUpOnNode = false;

                    foreach (var node in NodeCollection)
                    {
                        if (VisualTreeHelper.HitTest(node.Border, e.GetPosition(node)) != null)
                            mouseUpOnNode = true;
                    }


                    // if mouse up in empty space unselect all nodes
                    if (!mouseUpOnNode)
                    {
                        foreach (var node in SelectedNodes)
                            node.IsSelected = false;

                        SelectedNodes.Clear();
                    }


                    break;

                case MouseModes.Panning:

                    foreach (var node in NodeCollection)
                        MouseMove -= node.HostCanvas_MouseMove;


                    Console.WriteLine(NodeCollection.Count);

                    MouseMode = MouseModes.Nothing;
                    break;

                case MouseModes.Selection:
                    Children.Remove(selectionRectangle);
                    selectionRectangle = null;

                    MouseMode = MouseModes.Nothing;
                    break;
            }
        }

        /*
        public void DoDynamicAnimation(object sender, MouseButtonEventArgs args)
        {
            for (var i = 0; i < 36; ++i)
            {
                // var e = new Button { Width = 50, Height = 16, Content="Test" };

                //var e = new Ellipse { Width = 16, Height = 16, Fill = SystemColors.HighlightBrush };
                //var e = new Ellipse { Width = 6, Height = 6, Fill=Brushes.HotPink };

                var e = new SliderNode(this) {Left = Mouse.GetPosition(this).X, Top = Mouse.GetPosition(this).Y};

                // var e = new TextBlock { Text = "Test" };
                // var e = new Slider { Width = 100 };

                // var e = new ProgressBar { Width = 100 , Height =10, Value=i };

                // var e = =new DataGrid{Width=100, Height=100};
                // var e = new TextBox { Text = "Hallo" };
                // var e = new Label { Content = "Halllo" };
                // var e = new RadioButton { Content="dfsdf" };

                //Canvas.SetLeft(e, Mouse.GetPosition(this).X);
                //Canvas.SetTop(e, Mouse.GetPosition(this).Y);

                var tg = new TransformGroup();
                var translation = new TranslateTransform(30, 0);
                var translationName = "myTranslation" + translation.GetHashCode();
                RegisterName(translationName, translation);
                tg.Children.Add(translation);
                tg.Children.Add(new RotateTransform(i*10));
                e.RenderTransform = tg;

                NodeCollection.Add(e);
                Children.Add(e);

                var anim = new DoubleAnimation(3, 250, new Duration(new TimeSpan(0, 0, 0, 2, 0)))
                {
                    EasingFunction = new PowerEase {EasingMode = EasingMode.EaseOut}
                };

                var s = new Storyboard();
                Storyboard.SetTargetName(s, translationName);
                Storyboard.SetTargetProperty(s, new PropertyPath(TranslateTransform.YProperty));
                var storyboardName = "s" + s.GetHashCode();
                Resources.Add(storyboardName, s);

                s.Children.Add(anim);

                s.Completed +=
                    (sndr, evtArgs) =>
                    {
                        //panel.Children.Remove(e);
                        Resources.Remove(storyboardName);
                        UnregisterName(translationName);
                    };
                s.Begin();
            }
        }
        */


        public void VplControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:

                    foreach (var node in SelectedNodes)
                        node.Delete();

                    SelectedNodes.Clear();
                    break;
                case Key.C:
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        tempCollection = new TrulyObservableCollection<Node>();


                        foreach (var node in SelectedNodes)
                            tempCollection.Add(node);
                    }
                }
                    break;
                case Key.V:
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        if (tempCollection == null) return;
                        if (tempCollection.Count == 0) return;

                        var minLeft = double.MaxValue;
                        var minTop = double.MaxValue;
                        var maxLeft = double.MinValue;
                        var maxTop = double.MinValue;

                        foreach (var node in tempCollection)
                        {
                            if (node.Left < minLeft) minLeft = node.Left;
                            if (node.Top < minTop) minTop = node.Top;

                            if ((node.Left + node.ActualWidth) > maxLeft) maxLeft = node.Left + node.ActualWidth;
                            if ((node.Top + node.ActualHeight) > maxTop) maxTop = node.Top + node.ActualHeight;
                        }

                        var copyPoint = new Point(minLeft + (maxLeft - minLeft)/2, minTop + (maxTop - minTop)/2);
                        var pastePoint = Mouse.GetPosition(this);

                        var delta = Point.Subtract(pastePoint, copyPoint);

                        SelectedNodes.Clear();

                        var alreadyClonedConnectors = new List<Connector>();
                        var copyConnections = new List<CopyConnection>();

                        // copy nodes from clipboard to canvas
                        foreach (var node in tempCollection)
                        {
                            var newNode = node.Clone();

                            newNode.Left += delta.X;
                            newNode.Top += delta.Y;

                            newNode.Left = Convert.ToInt32(newNode.Left);
                            newNode.Top = Convert.ToInt32(newNode.Top);

                            NodeCollection.Add(newNode);

                            copyConnections.Add(new CopyConnection {NewNode = newNode, OldNode = node});
                        }

                        foreach (var cc in copyConnections)
                        {
                            var counter = 0;

                            foreach (var conn in cc.OldNode.InputPorts)
                            {
                                foreach (var connector in conn.ConnectedConnectors)
                                {
                                    if (!alreadyClonedConnectors.Contains(connector))
                                    {
                                        Connector newConnector = null;

                                        // start and end node are contained in selection
                                        if (tempCollection.Contains(connector.StartPort.ParentNode))
                                        {
                                            var cc2 =
                                                copyConnections.FirstOrDefault(
                                                    i => Equals(i.OldNode, connector.StartPort.ParentNode));

                                            if (cc2 != null)
                                            {
                                                newConnector = new Connector(this, cc2.NewNode.OutputPorts[0],
                                                    cc.NewNode.InputPorts[counter]);
                                            }
                                        }
                                        // only end node is contained in selection
                                        else
                                        {
                                            newConnector = new Connector(this, connector.StartPort,
                                                cc.NewNode.InputPorts[counter]);
                                        }

                                        if (newConnector != null)
                                        {
                                            alreadyClonedConnectors.Add(connector);
                                            ConnectorCollection.Add(newConnector);
                                        }
                                    }
                                }
                                counter++;
                            }
                        }
                    }
                }
                    break;
                case Key.G:
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (SelectedNodes.Count <= 1) return;

                        var nodeGroup = new NodeGroup(this)
                        {
                            ChildNodes =
                                NodeCollection.Where(node => SelectedNodes.Contains(node)).ToTrulyObservableCollection()
                        };
                    }
                }
                    break;
                case Key.S:
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        var saveFileDialog = new SaveFileDialog
                        {
                            Filter = "vplXML (.vplxml)|*.vplxml",
                            DefaultExt = "vplxml"
                        };

                        if (saveFileDialog.ShowDialog() == true)
                            SerializeNetwork(saveFileDialog.FileName);
                    }
                }
                    break;
                case Key.O:
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        var openFileDialog = new OpenFileDialog
                        {
                            Multiselect = false,
                            Filter = "vplXML (.vplxml)|*.vplxml"
                        };


                        if (openFileDialog.ShowDialog() == true)
                            DeserializeNetwork(openFileDialog.FileName);
                    }
                }
                    break;
                case Key.A:
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        SelectedNodes.Clear();

                        foreach (var node in NodeCollection)
                        {
                            node.IsSelected = true;
                            SelectedNodes.Add(node);
                        }
                    }
                }
                    break;
            }
        }

        public void VplControl_KeyDown(object sender, KeyEventArgs e)
        {
            var vector = new Vector();

            switch (e.Key)
            {
                case Key.Left:
                {
                    vector = new Vector(1, 0);
                }
                    break;
                case Key.Right:
                {
                    vector = new Vector(-1, 0);
                }
                    break;
                case Key.Up:
                {
                    vector = new Vector(0, 1);
                }
                    break;
                case Key.Down:
                {
                    vector = new Vector(0, -1);
                }
                    break;
            }


            double factor = 5;

            foreach (var node in NodeCollection)
            {
                node.Left += vector.X*factor;
                node.Top += vector.Y*factor;
            }
        }

        internal void SerializeNetwork(string filePath)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = false,
                Encoding = new UTF8Encoding()
            };

            StringWriter sb = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sb, settings))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("Document");

                xmlWriter.WriteStartElement("Nodes");

                foreach (var node in NodeCollection)
                {
                    xmlWriter.WriteStartElement(node.GetType().ToString());
                    node.SerializeNetwork(xmlWriter);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Connectors");
                foreach (var connector in ConnectorCollection)
                {
                    xmlWriter.WriteStartElement(connector.GetType().ToString());
                    connector.SerializeNetwork(xmlWriter);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        internal void DeserializeNetwork(string filePath)
        {
            NodeCollection.Clear();
            ConnectorCollection.Clear();
            Children.Clear();


            // Create an reader
            using (var reader = new XmlTextReader(filePath))
            {
                reader.MoveToContent();
                reader.ReadToDescendant("Nodes");

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            if (reader.Name != null)
                            {
                                var type = Type.GetType(reader.Name);

                                if (type == null) // try to find type in entry assembly
                                {
                                    try
                                    {
                                        var assembly = Assembly.GetEntryAssembly();
                                        type = assembly.GetTypes().First(t => t.FullName == reader.Name);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }


                                if (type != null)
                                {
                                    if (type.IsSubclassOf(typeof (Node)))
                                    {
                                        var node = (Node) Activator.CreateInstance(type, this);
                                        node.DeserializeNetwork(reader);
                                        NodeCollection.Add(node);
                                    }
                                    else if (type == typeof (Connector))
                                    {
                                        Node startNode = null;
                                        Node endNode = null;

                                        var value = reader.GetAttribute("StartNode");
                                        if (value != null)
                                        {
                                            startNode =
                                                NodeCollection.FirstOrDefault(node => node.Guid == new Guid(value));
                                        }

                                        value = reader.GetAttribute("EndNode");
                                        if (value != null)
                                            endNode = NodeCollection.FirstOrDefault(node => node.Guid == new Guid(value));

                                        value = reader.GetAttribute("StartIndex");
                                        var startIndex = Convert.ToInt32(value);

                                        value = reader.GetAttribute("EndIndex");
                                        var endIndex = Convert.ToInt32(value);


                                        if (startNode != null && endNode != null)
                                        {
                                            var startPort = startNode.OutputPorts[startIndex];
                                            var endPort = endNode.InputPorts[endIndex];

                                            if (startPort != null && endPort != null)
                                            {
                                                var connector = new Connector(this, startPort, endPort);
                                                ConnectorCollection.Add(connector);
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        case XmlNodeType.Text:

                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:

                            break;
                        case XmlNodeType.Comment:

                            break;
                        case XmlNodeType.EndElement:

                            break;
                    }
                }
            }
        }
    }


    public enum MouseModes
    {
        Nothing,
        Panning,
        Selection,
        GroupSelection
    }

    internal enum SplineModes
    {
        Nothing,
        First,
        Second
    }

    public enum NodeTypeModes
    {
        OnlyInternalTypes,
        OnlyExternalTypes,
        All
    }

    public enum GraphFlowDirections
    {
        Horizontal,
        Vertical
    }
}