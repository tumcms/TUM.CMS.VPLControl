using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Utilities;

namespace TUM.CMS.VplControl.Core
{
    public class Port : Control
    {
        private object data;

        public Port(string name, Node parent, PortTypes portType, Type type)
        {
            ParentNode = parent;

            DataType = type;
            PortType = portType;
            Name = name;

            if (portType == PortTypes.Input)
                Style = ParentNode.HostCanvas.FindResource("VplPortStyleLeft") as Style;
            else
                Style = ParentNode.HostCanvas.FindResource("VplPortStyleRight") as Style;

            MouseDown += Port_MouseDown;
            ParentNode.SizeChanged += ParentNode_SizeChanged;

            ParentNode.PropertyChanged += ParentNode_PropertyChanged;
            ConnectedConnectors = new List<Connector>();
            Origin = new BindingPoint(0, 0);
        }

        public string Text
        {
            get
            {
                if (Data != null)
                    return Name + " : " + DataType.Name + " : " + Data;
                return Name + " : " + DataType.Name + " : null";
            }
        }

        public new string Name { get; set; }
        public Ellipse Geometry { get; set; }
        public Node ParentNode { get; set; }
        public PortTypes PortType { get; set; }
        public Type DataType { get; set; }

        public object Data
        {
            get { return data; }
            set
            {
                CalculateData(value);
            }
        }

        public bool MultipleConnectionsAllowed
        {
            get; set; 
            
        }

        public BindingPoint Origin { get; set; }
        public List<Connector> ConnectedConnectors { get; set; }

        private void ParentNode_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalcOrigin();
        }

        private void ParentNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalcOrigin();
        }

        private void CalcOrigin()
        {
            Origin.X = TranslatePoint(new Point(Width/2, Height/2), ParentNode.HostCanvas).X;
            Origin.Y = TranslatePoint(new Point(Width/2, Height/2), ParentNode.HostCanvas).Y;
        }

        private void Port_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (ParentNode.HostCanvas.SplineMode)
            {
                case SplineModes.Nothing:
                    ParentNode.HostCanvas.TempStartPort = this;
                    ParentNode.HostCanvas.SplineMode = SplineModes.Second;
                    break;
                case SplineModes.Second:
                    if (
                        (
                            (
                                ParentNode.HostCanvas.TempStartPort.DataType.IsCastableTo(DataType) &&
                                ParentNode.HostCanvas.TypeSensitive && PortType == PortTypes.Output
                                ||
                                DataType.IsCastableTo(ParentNode.HostCanvas.TempStartPort.DataType) &&
                                ParentNode.HostCanvas.TypeSensitive && PortType == PortTypes.Input
                                ) // data types matching
                            ||
                            (!ParentNode.HostCanvas.TypeSensitive) // data types must not match
                            )
                        && PortType != ParentNode.HostCanvas.TempStartPort.PortType
                            // is not same port type --> input to output or output to input
                        && !Equals(ParentNode, ParentNode.HostCanvas.TempStartPort.ParentNode)) // is not same node
                    {
                        Connector connector;

                        if (PortType == PortTypes.Output)
                        {
                            if (ParentNode.HostCanvas.TempStartPort.ConnectedConnectors.Count > 0)
                            {
                                foreach (var tempConnector in ParentNode.HostCanvas.TempStartPort.ConnectedConnectors)
                                    tempConnector.RemoveFromCanvas();
                            }

                            connector = new Connector(ParentNode.HostCanvas, this, ParentNode.HostCanvas.TempStartPort);
                        }
                        else
                        {
                            if (ConnectedConnectors.Count > 0)
                            {
                                if (!MultipleConnectionsAllowed)
                                    foreach (var tempConnector in ConnectedConnectors)
                                        tempConnector.RemoveFromCanvas();              
                            }

                            connector = new Connector(ParentNode.HostCanvas, ParentNode.HostCanvas.TempStartPort, this);
                        }

                        ParentNode.HostCanvas.ConnectorCollection.Add(connector);
                    }


                    ParentNode.HostCanvas.SplineMode = SplineModes.Nothing;
                    ParentNode.HostCanvas.ClearTempLine();
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

        public void CalculateData(object value=null)
        {
            if (PortType == PortTypes.Input)
            {
                if (MultipleConnectionsAllowed && ConnectedConnectors.Count > 1)
                {
                    var listType = typeof (List<>).MakeGenericType(new Type[] {DataType});
                    IList list = (IList) Activator.CreateInstance(listType);

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