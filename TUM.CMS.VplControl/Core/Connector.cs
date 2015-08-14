using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

            endPort.Data = startPort.Data;
            startPort.DataChanged += endPort.StartPort_DataChanged;

            startPort.PositionChanged += startPort_PositionChanged;
            startPort.ParentNode.PropertyChanged += ParentNode_PropertyChanged;

            endPort.PositionChanged += endPort_PositionChanged;
            endPort.ParentNode.PropertyChanged += ParentNode_PropertyChanged;

            ObserveNode(StartPort.ParentNode);
            ObserveNode(EndPort.ParentNode);

            startPort.ConnectedConnectors.Add(this);
            endPort.ConnectedConnectors.Add(this);

            CalcPath();

            HostCanvas.Children.Add(Path);
            HostCanvas.Children.Add(srtEllipse);
            HostCanvas.Children.Add(endEllipse);
        }

        public Port StartPort { get; private set; }
        public Port EndPort { get; private set; }
        public VplControl HostCanvas { get; set; }
        public Path Path { get; set; }

        public Point StartBezierPoint
        {
            get { return StartPort.Origin + new Vector((StartPort.Origin - EndPort.Origin).Length/2, 0); }
        }

        public Point EndBezierPoint
        {
            get { return EndPort.Origin + new Vector(-(StartPort.Origin - EndPort.Origin).Length/2, 0); }
        }

        private void startPort_PositionChanged(object sender, EventArgs e)
        {
            CalcPath();
        }

        private void endPort_PositionChanged(object sender, EventArgs e)
        {
            CalcPath();
        }

        private void ParentNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalcPath();
        }

        private void CalcPath()
        {
            var spline = new BezierSegment(StartBezierPoint, EndBezierPoint, EndPort.Origin, true);

            var pColl = new PathSegmentCollection {spline};

            var pfColl = new PathFigureCollection {new PathFigure(StartPort.Origin, pColl, false)};

            Path.Data = new PathGeometry(pfColl);

            Canvas.SetLeft(srtEllipse, StartPort.Origin.X - srtEllipse.ActualWidth/2);
            Canvas.SetTop(srtEllipse, StartPort.Origin.Y - srtEllipse.ActualHeight/2);

            Canvas.SetLeft(endEllipse, EndPort.Origin.X - endEllipse.ActualWidth/2);
            Canvas.SetTop(endEllipse, EndPort.Origin.Y - endEllipse.ActualHeight/2);
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