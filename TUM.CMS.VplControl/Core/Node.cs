using System;
using System.Collections.Generic;
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
        private readonly int myid;
        private bool isResizeable;
        private bool isResizing;
        private int minMainHeight;
        private int minMainMinWidth;

        protected Node(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            Guid = Guid.NewGuid();

            id = Interlocked.Increment(ref id);
            id = Interlocked.Increment(ref id);
            myid = id;

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
            HostCanvas.MouseUp += HostCanvas_MouseUp;
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

            if (QuestButton != null)  if (QuestButton != null)base.QuestButton.Click += QuestButton_Click;

            SetZIndex(this, myid);
            SetZIndex(Border, myid);
            

            if (HitTestBorder != null) SetZIndex(HitTestBorder, myid);
            if (BinButton != null) SetZIndex(BinButton, myid);
            if (ResizeButton != null) SetZIndex(ResizeButton, myid);
            if (QuestButton != null) SetZIndex(QuestButton, myid);
            if (CaptionLabel != null) SetZIndex(CaptionLabel, myid);
            if (AutoCheckBox != null) SetZIndex(AutoCheckBox, myid);

            SetZIndex(TopComment, myid);
            SetZIndex(BottomComment, myid);
        }

        void QuestButton_Click(object sender, RoutedEventArgs e)
        {
            BottomComment.Visibility = BottomComment.Visibility== Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public int Id
        {
            get { return myid; }
        }

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

        public void HostCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnPropertyChanged("Left");
        }

        public void AddInputPortToNode(string name, Type type, bool multipleConnectionsAllowed=false)
        {
            var conn = new Port(name, this, PortTypes.Input, type) { MultipleConnectionsAllowed = multipleConnectionsAllowed };
            InputPortPanel.Children.Add(conn);
            conn.DataChanged += conn_DataChanged;
            InputPorts.Add(conn);
        }

        public void RemoveInputPortFromNode(Port conn)
        {
            foreach (var connector in conn.ConnectedConnectors)
                connector.RemoveFromCanvas();

            InputPortPanel.Children.Remove(conn);
            conn.DataChanged -= conn_DataChanged;
            InputPorts.Remove(conn);
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

        private void conn_DataChanged(object sender, EventArgs e)
        {
            try
            {
                if (AutoCheckBox.IsChecked != null && (bool) AutoCheckBox.IsChecked) Calculate();

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

        public void Delete()
        {
            HostCanvas.NodeCollection.Remove(this);
            HostCanvas.Children.Remove(Border);
            HostCanvas.Children.Remove(TopComment);
            HostCanvas.Children.Remove(BottomComment);

            Dispose();

            OnDeleted();
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
                    IsSelected = true;
                }
                else
                {
                    // Seleted this node as single
                    foreach (var node in HostCanvas.SelectedNodes)
                        node.IsSelected = false;

                    HostCanvas.SelectedNodes.Clear();

                    HostCanvas.SelectedNodes.Add(this);
                    IsSelected = true;

                    foreach (var node in HostCanvas.SelectedNodes)
                    {
                        HostCanvas.MouseMove += node.HostCanvas_MouseMove;
                        HostCanvas.MouseUp += node.Node_MouseUp;
                        node.OldMousePosition = e.GetPosition(HostCanvas);
                    }
                }
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

        public void Node_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isResizing = false;
            HostCanvas.MouseMove -= HostCanvas_MouseMove;
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

                if ((node.Left + node.ActualWidth) > maxLeft) maxLeft = node.Left + node.ActualWidth;
                if ((node.Top + node.ActualHeight) > maxTop) maxTop = node.Top + node.ActualHeight;
            }

            return new Rect(new Point(minLeft, minTop), new Size(maxLeft - minLeft, maxTop - minTop));
        }
    }
}