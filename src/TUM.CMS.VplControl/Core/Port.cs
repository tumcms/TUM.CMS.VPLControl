using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TUM.CMS.VplControl.Utilities;

namespace TUM.CMS.VplControl.Core
{
    public class Port : Control
    {
        private readonly VplControl hostCanvas;
        private object data;

        public Port(string name, Node parent, PortTypes portType, Type type)
        {
            ParentNode = parent;
            hostCanvas = ParentNode.HostCanvas;
            DataType = type;
            PortType = portType;
            Name = name;

            if (portType == PortTypes.Input)
                Style = hostCanvas.FindResource("VplPortStyleLeft") as Style;
            else
                Style = hostCanvas.FindResource("VplPortStyleRight") as Style;

            MouseDown += Port_MouseDown;

            ParentNode.SizeChanged += ParentNode_SizeChanged;
            ParentNode.PropertyChanged += ParentNode_PropertyChanged;

            ConnectedConnectors = new List<Connector>();
            Origin = new BindingPoint(0, 0);
        }

        public Port(string name, PortTypes portType, Type type, VplControl hostCanvas)
        {
            DataType = type;
            PortType = portType;
            Name = name;

            this.hostCanvas = hostCanvas;

            if (portType == PortTypes.Input)
                Style = this.hostCanvas.FindResource("VplPortStyleLeft") as Style;
            else
                Style = this.hostCanvas.FindResource("VplPortStyleRight") as Style;

            MouseDown += Port_MouseDown;

            // ParentNode.SizeChanged += ParentNode_SizeChanged;
            // ParentNode.PropertyChanged += ParentNode_PropertyChanged;

            ConnectedConnectors = new List<Connector>();
            Origin = new BindingPoint(0, 0);
        }

        public string Text
        {
            get
            {
                //if (Data != null)
                //    return Name + " : " + DataType.Name + " : " + Data;
                //return Name + " : " + DataType.Name + " : null";

                return Utilities.Utilities.DataToString(Data);
            }
        }

        public new string Name { get; set; }
        public Node ParentNode { get; set; }
        public PortTypes PortType { get; set; }
        public Type DataType { get; set; }

        public object Data
        {
            get { return data; }
            set { CalculateData(value); }
        }

        public bool MultipleConnectionsAllowed { get; set; }

        public BindingPoint Origin { get; set; }
        public List<Connector> ConnectedConnectors { get; set; }

        public void ParentNode_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalcOrigin();
        }

        public void ParentNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalcOrigin();
        }

        public void CalcOrigin()
        {
            Origin.X = TranslatePoint(new Point(Width/2, Height/2), hostCanvas).X;
            Origin.Y = TranslatePoint(new Point(Width/2, Height/2), hostCanvas).Y;
        }

        private void Port_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (hostCanvas.SplineMode)
            {
                case SplineModes.Nothing:

                    if (PortType== PortTypes.Input && !MultipleConnectionsAllowed && ConnectedConnectors.Count > 0)
                    {
                        Connector conn = ConnectedConnectors[0];
                        conn.Delete();
                        hostCanvas.TempStartPort = conn.StartPort;
                    }
                    else
                    {
                        hostCanvas.TempStartPort = this;
                    }

                    hostCanvas.SplineMode = SplineModes.Second;
                    break;
                case SplineModes.Second:
                    if (
                        (
                            hostCanvas.TempStartPort.DataType.IsCastableTo(DataType) &&
                            hostCanvas.TypeSensitive && PortType == PortTypes.Output
                            ||
                            DataType.IsCastableTo(hostCanvas.TempStartPort.DataType) &&
                            hostCanvas.TypeSensitive && PortType == PortTypes.Input // data types matching
                            ||
                            !hostCanvas.TypeSensitive // data types must not match
                            )
                        && PortType != hostCanvas.TempStartPort.PortType
                            // is not same port type --> input to output or output to input
                        && !Equals(ParentNode, hostCanvas.TempStartPort.ParentNode)) // is not same node
                    {
                        Connector connector;

                        if (PortType == PortTypes.Output)
                        {
                            if (hostCanvas.TempStartPort.ConnectedConnectors.Count > 0)
                            {
                                if (!hostCanvas.TempStartPort.MultipleConnectionsAllowed)
                                {
                                    foreach (var tempConnector in hostCanvas.TempStartPort.ConnectedConnectors)
                                        tempConnector.RemoveFromCanvas();

                                    hostCanvas.TempStartPort.ConnectedConnectors.Clear();
                                }
                            }

                            connector = new Connector(hostCanvas, this, hostCanvas.TempStartPort);
                        }
                        else
                        {
                            if (ConnectedConnectors.Count > 0)
                            {
                                if (!MultipleConnectionsAllowed)
                                {
                                    foreach (var tempConnector in ConnectedConnectors)
                                        tempConnector.RemoveFromCanvas();

                                    ConnectedConnectors.Clear();
                                }
                            }

                            connector = new Connector(hostCanvas, hostCanvas.TempStartPort, this);
                        }


                        connector.SynchroniseAfterZoom();
                        hostCanvas.ConnectorCollection.Add(connector);
                    }


                    hostCanvas.SplineMode = SplineModes.Nothing;
                    hostCanvas.ClearTempLine();
                    break;
            }

            e.Handled = true;
        }

        public void StartPort_DataChanged(object sender, EventArgs e)
        {
            CalculateData();
        }

        public event EventHandler DataChanged;

        public void OnDataChanged()
        {
            if (DataChanged != null)
                DataChanged(this, new EventArgs());
        }

        public void CalculateData(object value = null)
        {
            if (PortType == PortTypes.Input)
            {
                if (MultipleConnectionsAllowed && ConnectedConnectors.Count > 1)
                {
                    var listType = typeof (List<>).MakeGenericType(DataType);
                    var list = (IList) Activator.CreateInstance(listType);

                    foreach (var conn in ConnectedConnectors)
                    {
                        list.Add(conn.StartPort.Data);
                    }

                    data = list;
                }
                else if (ConnectedConnectors.Count > 0)
                {
                    data = ConnectedConnectors[0].StartPort.Data;
                }
                else
                {
                    data = null;
                }
            }
            else
            {
                data = value;
            }

            OnDataChanged();
        }
    }

    public enum PortTypes
    {
        Input,
        Output
    }
}