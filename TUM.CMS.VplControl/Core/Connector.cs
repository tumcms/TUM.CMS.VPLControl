using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.Core
{
    public class Connector
    {
        private readonly Ellipse endEllipse;
        private readonly Ellipse srtEllipse;

        public Connector(VplControl hostCanvas, Port startPort, Port endPort)
        {
            HostCanvas = hostCanvas;

            Path = new Path();

            srtEllipse = new Ellipse
            {
                Style = hostCanvas.FindResource("VplEllipseConnStyle") as Style
            };

            endEllipse = new Ellipse
            {
                Style = hostCanvas.FindResource("VplEllipseConnStyle") as Style
            };

            Panel.SetZIndex(Path, 2);
            Panel.SetZIndex(srtEllipse, startPort.ParentNode.Id + 1);
            Panel.SetZIndex(endEllipse, endPort.ParentNode.Id + 1);

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

            StartPort.ParentNode.PropertyChanged += Origin_PropertyChanged;
            EndPort.ParentNode.PropertyChanged += Origin_PropertyChanged;

            ObserveNode(StartPort.ParentNode);
            ObserveNode(EndPort.ParentNode);

            startPort.ConnectedConnectors.Add(this);
            endPort.ConnectedConnectors.Add(this);

            endPort.CalculateData(startPort.Data);

            DefinePath();

            HostCanvas.Children.Add(Path);
            HostCanvas.Children.Add(srtEllipse);
            HostCanvas.Children.Add(endEllipse);
        }

        public Port StartPort { get; private set; }
        public Port EndPort { get; private set; }
        public VplControl HostCanvas { get; set; }
        public Path Path { get; set; }
        public BindingPoint StartBezierPoint { get; set; }
        public BindingPoint EndBezierPoint { get; set; }

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
            RemoveFromCanvas();

            StartPort.ConnectedConnectors.Remove(this);
            EndPort.ConnectedConnectors.Remove(this);

            EndPort.CalculateData();
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
    }
}