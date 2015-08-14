using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace TUM.CMS.VplControl.Nodes
{
    public class ListNode : Node
    {
        public ListNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Insert(0, new ColumnDefinition());
            grid.ColumnDefinitions.Insert(1, new ColumnDefinition());


            var plusButton = new Button
            {
                Content = "+",
                Width = 60
            };

            plusButton.Click += plusButton_Click;

            var minusButton = new Button
            {
                Content = "-",
                Width = 60
            };

            minusButton.Click += minusButton_Click;

            SetColumn(plusButton, 0);
            SetColumn(minusButton, 1);

            grid.Children.Add(plusButton);
            grid.Children.Add(minusButton);

            AddControlToNode(grid);

            AddInputPortToNode("Item1", typeof (object));
            AddInputPortToNode("Item2", typeof (object));

            AddOutputPortToNode("List", typeof (object));
        }

        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputPorts.Count <= 1) return;
            RemoveInputPortFromNode(InputPorts.Last());
            Calculate();

            if (InputPorts.Count != 1) return;
            var grid = ControlElements[0] as Grid;
            if (grid != null) grid.Children[1].IsEnabled = false;
        }

        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            AddInputPortToNode("Item" + (OutputPorts.Count + 1), typeof (object));
            Calculate();

            if (InputPorts.Count <= 1) return;

            var grid = ControlElements[0] as Grid;
            if (grid != null) grid.Children[1].IsEnabled = true;
        }

        public override void Calculate()
        {
            var outputList = InputPorts.Select(conn => conn.Data).ToList();
            OutputPorts[0].Data = outputList;
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            xmlWriter.WriteStartAttribute("InputPortsCount");
            xmlWriter.WriteValue(InputPorts.Count);
            xmlWriter.WriteEndAttribute();
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            var value = xmlReader.GetAttribute("InputPortsCount");
            var connNumber = Convert.ToInt32(value);

            AddConns(connNumber);
        }

        private void AddConns(int connNumber)
        {
            if (connNumber == 1)
                RemoveInputPortFromNode(InputPorts.Last());
            else
            {
                for (var i = 0; i < connNumber - 2; i++)
                    AddInputPortToNode("Item" + (i + 1), typeof (object));
            }
        }

        public override Node Clone()
        {
            var node = new ListNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };

            node.AddConns(InputPorts.Count);

            return node;
        }
    }
}