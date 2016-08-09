using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using TUM.CMS.VplControl.Annotations;

namespace TUM.CMS.VplControl.Core
{
    public class Connector : INotifyPropertyChanged
    {
        public readonly ConnectorPort endEllipse;
        public readonly ConnectorPort srtEllipse;
        private int counter = 0;

        public Connector(VplControl hostCanvas, Port startPort, Port endPort)
        {
            HostCanvas = hostCanvas;

            Path = new Path();

            srtEllipse = new ConnectorPort(hostCanvas);
            endEllipse = new ConnectorPort(hostCanvas);

            Panel.SetZIndex(Path, 2);

            if (startPort.ParentNode != null)
            {
                Panel.SetZIndex(srtEllipse, startPort.ParentNode.Id + 1);
            }

            if (endPort.ParentNode != null)
            {
                Panel.SetZIndex(endEllipse, endPort.ParentNode.Id + 1);
            }


            Path.Style = HostCanvas.FindResource("VplConnectorStyle") as Style;

            StartPort = startPort;
            EndPort = endPort;

            Canvas.SetLeft(srtEllipse, StartPort.Origin.X - srtEllipse.ActualWidth/2);
            Canvas.SetTop(srtEllipse, StartPort.Origin.Y - srtEllipse.ActualHeight/2);

            Canvas.SetLeft(endEllipse, EndPort.Origin.X - endEllipse.ActualWidth/2);
            Canvas.SetTop(endEllipse, EndPort.Origin.Y - endEllipse.ActualHeight/2);

            StartBezierPoint = new BindingPoint(StartPort.Origin.X, StartPort.Origin.Y);
            EndBezierPoint = new BindingPoint(EndPort.Origin.X, EndPort.Origin.Y);

            startPort.DataChanged += endPort.StartPort_DataChanged;

            StartPort.Origin.PropertyChanged += Origin_PropertyChanged;
            EndPort.Origin.PropertyChanged += Origin_PropertyChanged;

            if (startPort.ParentNode != null)
            {
                StartPort.ParentNode.PropertyChanged += Origin_PropertyChanged;
                ObserveNode(StartPort.ParentNode);
            }

            if (endPort.ParentNode != null)
            {
                EndPort.ParentNode.PropertyChanged += Origin_PropertyChanged;
                ObserveNode(EndPort.ParentNode);
            }


            startPort.ConnectedConnectors.Add(this);
            endPort.ConnectedConnectors.Add(this);

            endPort.CalculateData(startPort.Data);

            DefinePath();
            HostCanvas.Children.Add(Path);

            Path.MouseDown += Path_MouseDown;
            Path.MouseUp += PathOnMouseUp;
        }

        private void PathOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            mouseButtonEventArgs.Handled = true;
        }

        private void Path_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            IsSelected = !IsSelected;
        }

        public Port StartPort { get; }
        public Port EndPort { get; }
        public VplControl HostCanvas { get; set; }
        public Path Path { get; set; }
        public BindingPoint StartBezierPoint { get; set; }
        public BindingPoint EndBezierPoint { get; set; }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    if (value)
                    {
                        Path.Style = HostCanvas.FindResource("VplSelectedConnectorStyle") as Style;
                        HostCanvas.SelectedConnectors.Add(this);
                    }
                    else
                    {
                        Path.Style = HostCanvas.FindResource("VplConnectorStyle") as Style;
                        HostCanvas.SelectedConnectors.Remove(this);
                    }
                }

                isSelected=value;


            }
        }



        public void SynchroniseAfterZoom()
        {
            Origin_PropertyChanged(null, null);
        }

        private void Origin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Point startBezierPoint, endBezierPoint;

            if (HostCanvas.GraphFlowDirection == GraphFlowDirections.Horizontal)
            {
                startBezierPoint = StartPort.Origin.Point +
                                   new Vector((StartPort.Origin.Point - EndPort.Origin.Point).Length/2, 0);
                endBezierPoint = EndPort.Origin.Point +
                                 new Vector(-(StartPort.Origin.Point - EndPort.Origin.Point).Length/2, 0);
            }
            else
            {
                startBezierPoint = StartPort.Origin.Point +
                                   new Vector(0, (StartPort.Origin.Point - EndPort.Origin.Point).Length/2);
                endBezierPoint = EndPort.Origin.Point +
                                 new Vector(0, -(StartPort.Origin.Point - EndPort.Origin.Point).Length/2);
            }

            StartBezierPoint.X = startBezierPoint.X;
            StartBezierPoint.Y = startBezierPoint.Y;

            EndBezierPoint.X = endBezierPoint.X;
            EndBezierPoint.Y = endBezierPoint.Y;

            srtEllipse.UpdateLayout();
             
            Canvas.SetLeft(srtEllipse, StartPort.Origin.X - srtEllipse.ActualWidth/2);
            Canvas.SetTop(srtEllipse, StartPort.Origin.Y - srtEllipse.ActualHeight/2);

            Canvas.SetLeft(endEllipse, EndPort.Origin.X - endEllipse.ActualWidth/2);
            Canvas.SetTop(endEllipse, EndPort.Origin.Y - endEllipse.ActualHeight/2);
        }

        private void DefinePath()
        {
            if (StartPort.Origin == null || EndPort.Origin == null) return;

            var spline = new BezierSegment {IsStroked = true};

            var b = new Binding("Point")
            {
                Source = StartBezierPoint,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(spline, BezierSegment.Point1Property, b);


            b = new Binding("Point")
            {
                Source = EndBezierPoint,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(spline, BezierSegment.Point2Property, b);


            b = new Binding("Point")
            {
                Source = EndPort.Origin,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(spline, BezierSegment.Point3Property, b);

            var pColl = new PathSegmentCollection {spline};

            var pFig = new PathFigure(StartPort.Origin.Point, pColl, false);

            b = new Binding("Point")
            {
                Source = StartPort.Origin,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(pFig, PathFigure.StartPointProperty, b);

            var pfColl = new PathFigureCollection {pFig};

            Path.Data = new PathGeometry(pfColl);
        }

        public void Delete()
        {
            RemoveFromCanvas();

            StartPort.ConnectedConnectors.Remove(this);
            EndPort.ConnectedConnectors.Remove(this);

            EndPort.CalculateData();
        }

        public void RemoveFromCanvas()
        {
            HostCanvas.Children.Remove(Path);
            HostCanvas.Children.Remove(srtEllipse);
            HostCanvas.Children.Remove(endEllipse);
            HostCanvas.ConnectorCollection.Remove(this);

            StartPort.DataChanged -= EndPort.StartPort_DataChanged;
        }

        private void ObserveNode(Node node)
        {
            node.DeletedInNodeCollection += node_DeletedInNodeCollection;
        }

        private void node_DeletedInNodeCollection(object sender, EventArgs e)
        {
            Delete();
        }

        public void SerializeNetwork(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartAttribute("StartNode");
            xmlWriter.WriteValue(StartPort.ParentNode.Guid.ToString());
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("StartIndex");
            xmlWriter.WriteValue(StartPort.ParentNode.OutputPorts.IndexOf(StartPort));
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("EndNode");
            xmlWriter.WriteValue(EndPort.ParentNode.Guid.ToString());
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("EndIndex");
            xmlWriter.WriteValue(EndPort.ParentNode.InputPorts.IndexOf(EndPort));
            xmlWriter.WriteEndAttribute();
        }

        public virtual void DeserializeNetwork(XmlReader xmlReader)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}