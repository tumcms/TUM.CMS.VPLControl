using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using TUM.CMS.VplControl.ContentMenu;
using TUM.CMS.VplControl.Themes;
using TUM.CMS.VplControl.Utilities;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.Windows.Shapes.Path;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace TUM.CMS.VplControl.Core
{
    public class VplControl : ZoomCanvas
    {
        private GraphFlowDirections graphFlowDirection;
        public GraphFlowDirections ImportFlowDirection;

        private RadialContentMenu radialMenu;
        private ScaleTransform scaleTransformZooming;
        private SelectionNode selectionNode;
        private Border selectionRectangle;
        private Point startSelectionRectanglePoint;
        private TrulyObservableCollection<Node> tempCollection;
        public Line TempLine;
        internal Port TempStartPort;

        public VplControl()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
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
                GraphFlowDirection = GraphFlowDirections.Vertical;

                KeyDown += VplControl_KeyDown;
                KeyUp += VplControl_KeyDown;

                ScaleTransform.Changed += ScaleTransformOnChanged;

                NodeCollection = new TrulyObservableCollection<Node>();
                NodeGroupCollection = new List<NodeGroup>();
                ConnectorCollection = new List<Connector>();
                SelectedNodes = new TrulyObservableCollection<Node>();
                SelectedConnectors = new TrulyObservableCollection<Connector>();
                ExternalNodeTypes = new List<Type>();

                TypeSensitive = true;

                InitializeGridBackground();
                InitializeTheme();
            }
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
        public TrulyObservableCollection<Connector> SelectedConnectors { get; set; }

        [Browsable(false)]
        public List<Type> ExternalNodeTypes { get; set; }

        [Browsable(false)]
        public NodeTypeModes NodeTypeMode { get; set; }

        public int ZoomIn { get; set; }
        public int ZoomOut { get; set; }

        [Category("All VPL settings")]
        [DisplayName(@"Type sensitive")]
        public bool TypeSensitive { get; set; }

        [Category("All VPL settings")]
        [DisplayName(@"Graph flow firection")]
        public GraphFlowDirections GraphFlowDirection
        {
            get { return graphFlowDirection; }
            set
            {
                if (graphFlowDirection == value) return;

                if (NodeCollection != null)
                {
                    var guid = new Guid();
                    SerializeNetwork(guid + ".vplxml");
                    graphFlowDirection = value;
                    DeserializeNetwork(guid + ".vplxml");
                    File.Delete(guid + ".vplxml");
                }
            }
        }

        [Category("All VPL settings")]
        [DisplayName(@"Graph flow firection")]
        [ExpandableObject]
        public Theme Theme { get; set; }

        private static void InitializeGridBackground()
        {
            var visualBrush = new VisualBrush
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
        }

        private void InitializeTheme()
        {
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
                Theme.TooltipBorderColor = tooltipBorderBrush.Color;

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

        private void TranslateTransformOnChanged()
        {
            foreach (var node in NodeCollection)
            {
                node.Left = node.Left;
            }
        }

        private void ScaleTransformOnChanged(object sender, EventArgs eventArgs)
        {
            if (Math.Abs(ScaleTransform.ScaleX - ScaleTransform.ScaleY) > 0.0001) return;

            //Theme.ConnectorThickness = 2*ScaleTransform.ScaleX;
            //Theme.PortSize = 20*ScaleTransform.ScaleX/2;
            //Theme.PortStrokeThickness = 1.5*ScaleTransform.ScaleX/2;
            //Theme.ConnEllipseSize = 14*ScaleTransform.ScaleX;
            //Theme.LineThickness = 1.5*ScaleTransform.ScaleX;

            foreach (var node in NodeCollection)
            {
                node.Left = node.Left;
            }
        }


        protected override async void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (mouseMode)
            {
                case MouseMode.Nothing:

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (e.ClickCount == 2)
                        {
                            // left double click in empty space of canvas
                            ShowSelectionNode();
                        }
                        else
                        {
                            // left singe click in empty space of canvas
                            startSelectionRectanglePoint = Mouse.GetPosition(this);
                            mouseMode = MouseMode.PreSelectionRectangle;
                            SplineMode = SplineModes.Nothing;
                        }

                        if (radialMenu != null)
                        {
                            radialMenu.IsOpen = false;
                            radialMenu.Dispose();
                            radialMenu = null;
                        }
                    }
                    else if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        start = e.GetPosition(this);
                        origin = new Point(TranslateTransform.X, TranslateTransform.Y);


                        HideElementsForTransformation();

                        Cursor = Cursors.Hand;
                        mouseMode = MouseMode.Panning;
                    }
                    else if (e.RightButton == MouseButtonState.Pressed)
                    {
                        if (radialMenu == null)
                        {
                            radialMenu = new RadialContentMenu(this);
                            Children.Add(radialMenu);
                        }

                        if (radialMenu.IsOpen)
                        {
                            radialMenu.IsOpen = false;
                            await Task.Delay(400);
                        }
                        radialMenu.SetValue(LeftProperty, Mouse.GetPosition(this).X - 150);
                        radialMenu.SetValue(TopProperty, Mouse.GetPosition(this).Y - 150);

                        radialMenu.IsOpen = true;
                    }

                    break;
                case MouseMode.PreSelectionRectangle:
                    if (e.ClickCount == 2)
                    {
                        // left double click in empty space of canvas
                        ShowSelectionNode();
                    }
                    break;
                case MouseMode.Selection:

                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        // left singe click in empty space of canvas while nodes are selected
                        UnselectAllElements();
                        mouseMode = MouseMode.Nothing;
                    }
                    break;

                case MouseMode.GroupSelection:
                    break;
            }
        }

        private void ShowSelectionNode()
        {
            if (selectionNode == null)
            {
                selectionNode = new SelectionNode(this);
            }

            selectionNode.Left = Mouse.GetPosition(this).X - 45;
            selectionNode.Top = Mouse.GetPosition(this).Y - 20;

            selectionNode.Show();
        }

        private void HideElementsForTransformation()
        {
            foreach (var node in NodeCollection)
            {
                node.ContentGrid.Visibility = Visibility.Collapsed;
                node.HitTestGrid.Visibility = Visibility.Collapsed;
            }

            foreach (var conn in ConnectorCollection)
            {
                conn.Path.Visibility = Visibility.Collapsed;
                conn.srtEllipse.Visibility = Visibility.Collapsed;
                conn.endEllipse.Visibility = Visibility.Collapsed;
            }
        }

        private void UnselectAllElements()
        {
            foreach (var node in SelectedNodes)
            {
                node.IsSelected = false;
            }

            foreach (var conn in ConnectorCollection)
            {
                conn.IsSelected = false;
            }

            SelectedNodes.Clear();
            SelectedConnectors.Clear();
            SelectedUiElements.Clear();
        }

        protected override void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseMode == MouseMode.PreSelectionRectangle || mouseMode == MouseMode.SelectionRectangle)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    mouseMode = MouseMode.SelectionRectangle;

                    if (selectionRectangle == null)
                    {
                        selectionRectangle = new Border
                        {
                            // Move to WPF style
                            Background = Brushes.Transparent,
                            BorderBrush =
                                new SolidColorBrush(Application.Current.Resources["ColorBlue"] is Color
                                    ? (Color) Application.Current.Resources["ColorBlue"]
                                    : new Color()),
                            CornerRadius = new CornerRadius(5),
                            BorderThickness = new Thickness(2)
                        };

                        SetLeft(selectionRectangle, startSelectionRectanglePoint.X);
                        SetTop(selectionRectangle, startSelectionRectanglePoint.Y);

                        Children.Add(selectionRectangle);
                    }

                    var currentPosition = Mouse.GetPosition(this);
                    var delta = Point.Subtract(currentPosition, startSelectionRectanglePoint);

                    if (delta.X < 0)
                        SetLeft(selectionRectangle, currentPosition.X);

                    if (delta.Y < 0)
                        SetTop(selectionRectangle, currentPosition.Y);

                    selectionRectangle.Width = Math.Abs(delta.X);
                    selectionRectangle.Height = Math.Abs(delta.Y);

                    foreach (var node in NodeCollection)
                    {
                        SelectedNodes.Remove(node);

                        if (node.Left >= GetLeft(selectionRectangle) &&
                            node.Left + node.ActualWidth <= GetLeft(selectionRectangle) + selectionRectangle.Width &&
                            node.Top >= GetTop(selectionRectangle) &&
                            node.Top + node.ActualHeight <= GetTop(selectionRectangle) + selectionRectangle.Height)
                        {
                            node.IsSelected = true;
                            SelectedNodes.Add(node);
                        }
                        else
                            node.IsSelected = false;
                    }
                }
            }
            else if (mouseMode == MouseMode.Panning)
            {
                var v = start - e.GetPosition(this);

                TranslateTransform.X = origin.X - v.X;
                TranslateTransform.Y = origin.Y - v.Y;
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

        protected override void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl)) return;

            mouseMode = MouseMode.Zooming;

            var zoom = e.Delta > 0 ? .2 : -.2;

            if (!(e.Delta > 0) && (ScaleTransform.ScaleX < .4 || ScaleTransform.ScaleY < .4))
                return;

            var elementsToZoom = new List<UIElement>();
            elementsToZoom.AddRange(Children.OfType<Border>());
            elementsToZoom.AddRange(Children.OfType<Ellipse>());
            elementsToZoom.AddRange(Children.OfType<Path>());

            foreach (var element in elementsToZoom)
            {
                element.UpdateLayout();

                var position = e.GetPosition(element);
                double width = 0;
                double height = 0;

                if (element is Border)
                {
                    var border = element as Border;

                    width = border.ActualWidth;
                    height = border.ActualHeight;
                }
                else if (element is Ellipse)
                {
                    var ellipse = element as Ellipse;

                    width = ellipse.ActualWidth;
                    height = ellipse.ActualHeight;
                }
                else if (element is Path)
                {
                    var path = element as Path;

                    width = path.ActualWidth;
                    height = path.ActualHeight;
                }

                if (width > 0 && height > 0)
                {
                    element.RenderTransformOrigin = new Point(position.X/width, position.Y/height);
                }
            }

            ScaleTransform.ScaleX += zoom;
            ScaleTransform.ScaleY += zoom;

            mouseMode = MouseMode.Nothing;
        }


        protected override void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (mouseMode)
            {
                case MouseMode.Nothing:

                    var mouseUpOnNode = false;

                    foreach (var node in NodeCollection)
                    {
                        if (VisualTreeHelper.HitTest(node.Border, e.GetPosition(node)) != null)
                            mouseUpOnNode = true;
                    }

                    // if mouse up in empty space unselect all nodes
                    if (!mouseUpOnNode && e.ChangedButton != MouseButton.Right)
                    {
                        UnselectAllElements();
                    }
                    break;

                case MouseMode.Panning:

                    ShowElementsAfterTransformation();

                    Cursor = Cursors.Arrow;
                    mouseMode = MouseMode.Nothing;

                    break;

                case MouseMode.PreSelectionRectangle:
                    UnselectAllElements();
                    mouseMode = MouseMode.Nothing;
                    break;
                case MouseMode.SelectionRectangle:
                    Children.Remove(selectionRectangle);
                    selectionRectangle = null;

                    mouseMode = SelectedNodes.Count > 0 ? MouseMode.Selection : MouseMode.Nothing;

                    break;
            }
        }

        private void ShowElementsAfterTransformation()
        {
            foreach (var node in NodeCollection)
            {
                node.ContentGrid.Visibility = Visibility.Hidden;
                node.HitTestGrid.Visibility = Visibility.Hidden;
            }

            foreach (var conn in ConnectorCollection)
            {
                conn.Path.Visibility = Visibility.Hidden;
                conn.srtEllipse.Visibility = Visibility.Hidden;
                conn.endEllipse.Visibility = Visibility.Hidden;
            }

            ScaleTransform.ScaleX += 1;
            ScaleTransform.ScaleX -= 1;

            foreach (var node in NodeCollection)
            {
                node.ContentGrid.Visibility = Visibility.Visible;
                node.HitTestGrid.Visibility = Visibility.Visible;
            }

            foreach (var conn in ConnectorCollection)
            {
                conn.Path.Visibility = Visibility.Visible;
                conn.srtEllipse.Visibility = Visibility.Visible;
                conn.endEllipse.Visibility = Visibility.Visible;
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

                    foreach (var conn in SelectedConnectors)
                    {
                        conn.Delete();
                    }

                    SelectedNodes.Clear();
                    SelectedConnectors.Clear();
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

                        var bBox = Node.GetBoundingBoxOfNodes(tempCollection.ToList());

                        var copyPoint = new Point(bBox.Left + bBox.Size.Width/2, bBox.Top + bBox.Size.Height/2);
                        var pastePoint = Mouse.GetPosition(this);

                        var delta = Point.Subtract(pastePoint, copyPoint);

                        UnselectAllElements();

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

                            newNode.Show();

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
                        GroupNodes();
                }
                    break;

                case Key.S:
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        SaveFile();
                }
                    break;
                case Key.T:
                {
                    Console.WriteLine("T");
                    foreach (var node in NodeCollection)
                    {
                        Console.WriteLine(node.ActualWidth);
                        Console.WriteLine(node.ActualHeight);
                    }
                }
                    break;
                case Key.O:
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        OpenFile();
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
                case Key.Escape:
                {
                    UnselectAllElements();
                    mouseMode = MouseMode.Nothing;
                }
                    break;
                case Key.LeftCtrl:
                    if (!Keyboard.IsKeyDown(Key.RightCtrl)) ShowElementsAfterTransformation();
                    break;
                case Key.RightCtrl:
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl)) ShowElementsAfterTransformation();
                    break;
            }
        }

        public void NewFile()
        {
            NodeCollection.Clear();
            ConnectorCollection.Clear();
            Children.Clear();
            if (radialMenu != null) radialMenu.Dispose();
            radialMenu = null;
            selectionNode = null;
        }

        public void OpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "vplXML (.vplxml)|*.vplxml"
            };


            if (openFileDialog.ShowDialog() == true)
                DeserializeNetwork(openFileDialog.FileName);
        }

        public void OpenFile(string filepath)
        {
            DeserializeNetwork(filepath);
        }

        public void SaveFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "vplXML (.vplxml)|*.vplxml",
                DefaultExt = "vplxml"
            };

            if (saveFileDialog.ShowDialog() == true)
                SerializeNetwork(saveFileDialog.FileName);
        }

        public void GroupNodes()
        {
            if (SelectedNodes.Count <= 1) return;

            var nodeGroup = new NodeGroup(this)
            {
                ChildNodes =
                    NodeCollection.Where(node => SelectedNodes.Contains(node)).ToTrulyObservableCollection()
            };
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
                case Key.LeftCtrl:
                    HideElementsForTransformation();
                    break;
                case Key.RightCtrl:
                    HideElementsForTransformation();
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

                xmlWriter.WriteStartAttribute("GraphFlowDirection");
                xmlWriter.WriteValue(GraphFlowDirection.ToString());
                xmlWriter.WriteEndAttribute();

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
            var tempNodeCollection = new List<Node>();

            NewFile();

            // Create an reader
            using (var reader = new XmlTextReader(filePath))
            {
                reader.MoveToContent();

                var enumString = reader.GetAttribute("GraphFlowDirection");

                if (enumString != null)
                {
                    ImportFlowDirection =
                        (GraphFlowDirections) Enum.Parse(typeof (GraphFlowDirections), enumString, true);
                }


                reader.ReadToDescendant("Nodes");

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            if (reader.Name != null)
                            {
                                var type = Type.GetType(reader.Name);

                                if (type == null)
                                {
                                    try // try to find type in entry assembly
                                    {
                                        var assembly = Assembly.GetEntryAssembly();
                                        type = assembly.GetTypes().FirstOrDefault(t => t.FullName == reader.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }

                                    try // try to find type in ExternalNodeTypes
                                    {
                                        type = ExternalNodeTypes.ToArray()
                                            .FirstOrDefault(t => t.FullName == reader.Name);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                }


                                if (type != null)
                                {
                                    if (type.IsSubclassOf(typeof (Node)))
                                    {
                                        var node = (Node) Activator.CreateInstance(type, this);
                                        node.DeserializeNetwork(reader);

                                        tempNodeCollection.Add(node);
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


            foreach (var node in tempNodeCollection)
            {
                node.Show();
            }
        }
    }


    public enum MouseMode
    {
        Nothing,
        Panning,
        Selection,
        GroupSelection,
        PreSelectionRectangle,
        SelectionRectangle,
        Zooming
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