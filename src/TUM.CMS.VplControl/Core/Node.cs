using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace TUM.CMS.VplControl.Core
{
    public abstract class Node : VplElement
    {
        private static readonly Action emptyDelegate = delegate { };
        private static int id = 3;
        private bool isResizeable;
        private bool isResizing;
        private int minMainHeight;
        private int minMainMinWidth;

        protected Node(VplControl hostCanvas) : base(hostCanvas)
        {
            Guid = Guid.NewGuid();

            id = Interlocked.Increment(ref id);
            id = Interlocked.Increment(ref id);
            Id = id;

            InputPorts = new List<Port>();
            OutputPorts = new List<Port>();
            ControlElements = new List<UIElement>();

            IsHitTestVisible = true;
            HasError = false;


            SpaceCanvas = new Canvas();
            Children.Add(ContentGrid = new Grid {ShowGridLines = false, Background = Brushes.Transparent});


            if (hostCanvas.GraphFlowDirection == GraphFlowDirections.Horizontal)
            {
                // ----------------------------------------------------------------------------------------------------------------------
                // Content Panels
                // ----------------------------------------------------------------------------------------------------------------------
                InputPortPanel = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center
                };

                SetColumn(InputPortPanel, 0);
                SetRow(InputPortPanel, 1);
                ContentGrid.Children.Add(InputPortPanel);

                OutputPortPanel = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center
                };
                SetColumn(OutputPortPanel, 2);
                SetRow(OutputPortPanel, 1);
                ContentGrid.Children.Add(OutputPortPanel);
            }
            else
            {
                // ----------------------------------------------------------------------------------------------------------------------
                // Content Panels
                // ----------------------------------------------------------------------------------------------------------------------
                InputPortPanel = new DockPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                SetRow(InputPortPanel, 0);
                SetColumn(InputPortPanel, 1);
                ContentGrid.Children.Add(InputPortPanel);

                OutputPortPanel = new DockPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                SetColumn(OutputPortPanel, 1);
                SetRow(OutputPortPanel, 2);
                ContentGrid.Children.Add(OutputPortPanel);
            }

            // ----------------------------------------------------------------------------------------------------------------------
            // Content grid row and column definitions
            // ----------------------------------------------------------------------------------------------------------------------
            ContentGrid.ColumnDefinitions.Insert(0,
                new ColumnDefinition {Width = new GridLength(1, GridUnitType.Auto)});
            // Input
            ContentGrid.ColumnDefinitions.Insert(0,
                new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
            // Content
            ContentGrid.ColumnDefinitions.Insert(0,
                new ColumnDefinition {Width = new GridLength(1, GridUnitType.Auto)});
            // Output

            ContentGrid.RowDefinitions.Insert(0, new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});
            // Header
            ContentGrid.RowDefinitions.Insert(1, new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});
            // Content
            ContentGrid.RowDefinitions.Insert(1, new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});
            // Footer
            ContentGrid.RowDefinitions.Insert(1, new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});
            // Risize area

            ContentGrid.SizeChanged += ContentGridOnSizeChanged;
            ContentGrid.VerticalAlignment = VerticalAlignment.Center;

            // ----------------------------------------------------------------------------------------------------------------------
            // Main content grid
            // ----------------------------------------------------------------------------------------------------------------------
            MainContentGrid = new Grid
            {
                ShowGridLines = false,
                Style = FindResource("MainContentGridStyle") as Style
            };


            SetColumn(MainContentGrid, 1);
            SetRow(MainContentGrid, 1);
            ContentGrid.Children.Add(MainContentGrid);

            // ----------------------------------------------------------------------------------------------------------------------
            // Event delagates
            // ----------------------------------------------------------------------------------------------------------------------
            Border.MouseDown += Node_MouseDown;

            Loaded += Node_Loaded;
            KeyUp += Node_KeyUp;
            KeyDown += Node_KeyDown;

            // ----------------------------------------------------------------------------------------------------------------------
            // Comments
            // ----------------------------------------------------------------------------------------------------------------------
            TopComment = new Comment(this)
            {
                Text =
                    "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.",
                Background = HostCanvas.FindResource("CommentBackgroundBrushError") as Brush,
                ExpandSide = CommentExpandSides.Top
            };


            BottomComment = new Comment(this)
            {
                Text =
                    "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.",
                Background = HostCanvas.FindResource("CommentBackgroundBrush") as Brush,
                ExpandSide = CommentExpandSides.Bottom
            };

            TopComment.Visibility = Visibility.Collapsed;
            BottomComment.Visibility = Visibility.Collapsed;

            ShowHelpOnMouseOver = false;

            if (QuestButton != null) if (QuestButton != null) QuestButton.Click += QuestButton_Click;

            SetZIndex(this, Id);
            SetZIndex(Border, Id);


            if (HitTestBorder != null) SetZIndex(HitTestBorder, Id);
            if (BinButton != null) SetZIndex(BinButton, Id);
            if (ResizeButton != null) SetZIndex(ResizeButton, Id);
            if (QuestButton != null) SetZIndex(QuestButton, Id);
            if (CaptionLabel != null) SetZIndex(CaptionLabel, Id);
            if (AutoCheckBox != null) SetZIndex(AutoCheckBox, Id);

            SetZIndex(TopComment, Id);
            SetZIndex(BottomComment, Id);

            if (GetType() == typeof (SelectionNode)) return;
            HostCanvas.NodeCollection.Add(this);
        }

        private void ContentGridOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if(HostCanvas.mouseMode== MouseMode.Zooming) return;

            if (ActualWidth > 0)
                HitTestBorder.Width = ActualWidth + 30;

            if (sizeChangedEventArgs.WidthChanged)
            {

                Border.Width = Math.Ceiling(ContentGrid.ActualWidth / 10.0d) * 10 +10;

                List<Double> values = new List<Double>()
                {
                    Border.ActualWidth/1.618, //Golden Ratio
                    Border.ActualWidth*1.618, //Golden Ratio flip
                    Border.ActualWidth, //Quadratic
                    Border.ActualWidth / 10, //1:10
                    Border.ActualWidth / 5, //1:5
                };

                for (int i = 0; i < values.Count; i++)
                {
                    values[i] = Math.Ceiling(values[i] / 10.0d)*10;
                }

                

                //Console.Write(GetType() + @": ActualContentGridWidth: " + ContentGrid.ActualWidth + @" - ActualContentGridHeight: " + ContentGrid.ActualHeight + " ---- ");

                foreach (var value in values)
                {
                    //Console.Write(value + @", ");
                }

                //Console.WriteLine(@"Min: " + values.Min());

                values = values.Where(v => v > ContentGrid.ActualHeight ).ToList();

                if (values.Count > 0)
                {
                    Border.MinHeight = values.Min();
                   // ContentGrid.Height = values.Min();
                    
                }
            }
        }

        public int Id { get; }

        public bool HasError { get; set; }
        public Guid Guid { get; set; }
        public Canvas SpaceCanvas { get; set; }
        public string NodeCaption { get; set; }
        public Grid ContentGrid { get; set; }
        public Grid MainContentGrid { get; set; }
        public Panel InputPortPanel { get; set; }
        public Panel OutputPortPanel { get; set; }
        public Comment TopComment { get; set; }
        public Comment BottomComment { get; set; }
        public bool ShowHelpOnMouseOver { get; set; }
        public Point OldMousePosition { get; set; }
        public List<Port> InputPorts { get; set; }
        public List<Port> OutputPorts { get; set; }
        public List<UIElement> ControlElements { get; set; }

        public bool IsResizeable
        {
            get { return isResizeable; }
            set
            {
                if (value == isResizeable) return;
                isResizeable = value;

                // ----------------------------------------------------------------------------------------------------------------------
                // Risize button
                // ----------------------------------------------------------------------------------------------------------------------
                var resizeRectangle = new Border();
                resizeRectangle.Width = 20;
                resizeRectangle.Height = 20;
                resizeRectangle.BorderBrush = Brushes.LightGray;
                resizeRectangle.BorderThickness = new Thickness(1);
                resizeRectangle.Background = Border.Background;
                resizeRectangle.CornerRadius = new CornerRadius(2);
                resizeRectangle.HorizontalAlignment = HorizontalAlignment.Center;


                resizeRectangle.MouseDown += resizeRectangle_MouseDown;
                resizeRectangle.MouseLeave += resizeRectangle_MouseLeave;
                resizeRectangle.MouseUp += resizeRectangle_MouseUp;

                SetColumn(resizeRectangle, 2);
                SetRow(resizeRectangle, 2);
                ContentGrid.Children.Add(resizeRectangle);
            }
        }

        private void QuestButton_Click(object sender, RoutedEventArgs e)
        {
            BottomComment.Visibility = BottomComment.Visibility == Visibility.Collapsed
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void Node_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = false;
        }

        private void Node_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = false;
        }

        private void resizeRectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            HostCanvas.MouseMove -= HostCanvas_MouseMove;
            isResizing = false;
            e.Handled = true;
        }

        private void resizeRectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HostCanvas.MouseMove -= HostCanvas_MouseMove;
            isResizing = false;
            e.Handled = true;
        }

        private void resizeRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OldMousePosition = e.GetPosition(HostCanvas);
            isResizing = true;
            HostCanvas.MouseMove += HostCanvas_MouseMove;
            e.Handled = true;
        }

        private void Node_Loaded(object sender, RoutedEventArgs e)
        {
            if (GetType() == typeof (SelectionNode)) return;

            AutoCheckBox.Checked += autoCheckBox_Checked;
        }

        private void autoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Calculate();
        }


        public void AddInputPortToNode(string name, Type type, bool multipleConnectionsAllowed = false)
        {
            var port = new Port(name, this, PortTypes.Input, type)
            {
                MultipleConnectionsAllowed = multipleConnectionsAllowed
            };
            InputPortPanel.Children.Add(port);
            port.DataChanged += port_DataChanged;
            InputPorts.Add(port);
        }

        public void RemoveInputPortFromNode(Port port)
        {
            foreach (var connector in port.ConnectedConnectors)
                connector.RemoveFromCanvas();

            InputPortPanel.Children.Remove(port);
            port.DataChanged -= port_DataChanged;
            InputPorts.Remove(port);
        }

        public void RemoveAllInputPortsFromNode(List<string> without = null)
        {
            if (without == null)
            {
                while (InputPorts.Any())
                    RemoveInputPortFromNode(InputPorts.First());
            }
            else
            {
                var filteredPorts = InputPorts.Where(port => !without.Contains(port.Name)).ToList();

                foreach (var port in filteredPorts)
                    RemoveInputPortFromNode(port);
            }
        }

        public void AddOutputPortToNode(string name, Type type)
        {
            var conn = new Port(name, this, PortTypes.Output, type) {MultipleConnectionsAllowed = true};
            OutputPortPanel.Children.Add(conn);

            OutputPorts.Add(conn);
        }

        public void AddControlToNode(UIElement element)
        {
            AddChildControlToMainContentGrid(element);
            ControlElements.Add(element);
        }

        private void port_DataChanged(object sender, EventArgs e)
        {
            try
            {
                if (AutoCheckBox.IsChecked != null && (bool) AutoCheckBox.IsChecked)
                    Calculate();

                HasError = false;
                TopComment.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                HasError = true;

                TopComment.Text = ex.ToString();
                TopComment.Visibility = Visibility.Visible;
            }
        }

        public abstract void Calculate();
        public event EventHandler DeletedInNodeCollection;

        public void Delete(bool removeConnectors = true)
        {
            HostCanvas.NodeCollection.Remove(this);
            HostCanvas.Children.Remove(Border);
            HostCanvas.Children.Remove(TopComment);
            HostCanvas.Children.Remove(BottomComment);

            Dispose();

            if (removeConnectors) OnDeleted();
        }

        public override void binButton_Click(object sender, RoutedEventArgs e)
        {
            base.binButton_Click(sender, e);

            Delete();
        }

        private void AddChildControlToMainContentGrid(UIElement control)
        {
            MainContentGrid.RowDefinitions.Insert(MainContentGrid.Children.Count, new RowDefinition());
            MainContentGrid.Children.Add(control);

            SetRow(control, MainContentGrid.Children.Count - 1);
            SetColumn(control, 1);
        }

        public virtual void Node_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (HostCanvas.SelectedNodes.Contains(this))
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    // Remove this node from selection
                    HostCanvas.SelectedNodes.Remove(this);
                    HostCanvas.SelectedUiElements.Remove(Border);
                    IsSelected = false;
                }
                else
                {
                    // Subsribe multiselection to hostCanvas MouseMove event
                    foreach (var node in HostCanvas.SelectedNodes)
                    {
                        HostCanvas.MouseMove += node.HostCanvas_MouseMove;
                        HostCanvas.MouseUp += node.Node_MouseUp;
                        node.OldMousePosition = e.GetPosition(HostCanvas);
                    }
                }
            }
            else
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    // add this node to selection
                    HostCanvas.SelectedNodes.Add(this);
                    HostCanvas.SelectedUiElements.Add(Border);
                    IsSelected = true;
                }
                else
                {
                    // Seleted this node as single
                    foreach (var node in HostCanvas.SelectedNodes)
                        node.IsSelected = false;

                    HostCanvas.SelectedNodes.Clear();
                    HostCanvas.SelectedUiElements.Clear();

                    HostCanvas.SelectedNodes.Add(this);
                    HostCanvas.SelectedUiElements.Add(Border);
                    IsSelected = true;

                    foreach (var node in HostCanvas.SelectedNodes)
                    {
                        HostCanvas.MouseMove += node.HostCanvas_MouseMove;
                        HostCanvas.MouseUp += node.Node_MouseUp;
                        node.OldMousePosition = e.GetPosition(HostCanvas);
                    }
                }

                HostCanvas.mouseMode = MouseMode.Selection;
            }

            e.Handled = true;
        }

        public void HostCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(HostCanvas);
            var delta = p - OldMousePosition;

            if (isResizing)
            {
                MainContentGrid.MinWidth = MainContentGrid.ActualWidth + delta.X;
                MainContentGrid.Height = MainContentGrid.ActualHeight + delta.Y;

                HitTestBorder.Width = ActualWidth + 10;
                HitTestBorder.Height = 30;
            }
            else
            {
                Left += delta.X;
                Top += delta.Y;
            }

            OldMousePosition = p;
        }


        public override void OnSelectionChanged(object sender, EventArgs e)
        {
            base.OnSelectionChanged(sender, e);

            foreach (var conn in InputPorts.Concat(OutputPorts).SelectMany(port => port.ConnectedConnectors))
            {
                conn.IsSelected = IsSelected;
            }
        }


        public void Node_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isResizing = false;
            HostCanvas.MouseMove -= HostCanvas_MouseMove;
            HostCanvas.MouseUp -= Node_MouseUp;
        }

        public void OnDeleted()
        {
            if (DeletedInNodeCollection != null)
                DeletedInNodeCollection(this, new EventArgs());
        }

        public virtual void SerializeNetwork(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartAttribute("GUID");
            xmlWriter.WriteValue(Guid.ToString());
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("TOP");
            xmlWriter.WriteValue(Top);
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("Left");
            xmlWriter.WriteValue(Left);
            xmlWriter.WriteEndAttribute();
        }

        public virtual void DeserializeNetwork(XmlReader xmlReader)
        {
            var value = xmlReader.GetAttribute("GUID");
            if (value != null) Guid = new Guid(value);

            var topValue = xmlReader.GetAttribute("TOP").Replace(",", ".");
            var leftValue = xmlReader.GetAttribute("Left").Replace(",", ".");

            topValue = xmlReader.GetAttribute("TOP").Replace(".", ",");
            leftValue = xmlReader.GetAttribute("Left").Replace(".", ",");

            if (HostCanvas.ImportFlowDirection == HostCanvas.GraphFlowDirection)
            {
                Top = Convert.ToDouble(topValue);
                Left = Convert.ToDouble(leftValue);
            }
            else
            {
                Left = Convert.ToDouble(topValue);
                Top = Convert.ToDouble(leftValue);
            }
        }

        public abstract Node Clone();

        public static Rect GetBoundingBoxOfNodes(List<Node> nodes)
        {
            if (nodes.Count == 0) return new Rect();

            var minLeft = double.MaxValue;
            var minTop = double.MaxValue;
            var maxLeft = double.MinValue;
            var maxTop = double.MinValue;

            foreach (var node in nodes)
            {
                if (node.Left < minLeft) minLeft = node.Left;
                if (node.Top < minTop) minTop = node.Top;

                if (node.Left + node.ActualWidth > maxLeft) maxLeft = node.Left + node.ActualWidth;
                if (node.Top + node.ActualHeight > maxTop) maxTop = node.Top + node.ActualHeight;
            }

            return new Rect(new Point(minLeft, minTop), new Size(maxLeft - minLeft, maxTop - minTop));
        }
    }
}