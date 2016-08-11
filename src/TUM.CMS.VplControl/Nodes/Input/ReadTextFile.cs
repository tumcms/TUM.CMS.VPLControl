using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Input
{
    public class ReadTextFileNode : Node
    {
        private readonly TextBlock textBlock;

        public ReadTextFileNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddInputPortToNode("String", typeof(string));
            AddOutputPortToNode("String", typeof (string));

            var label = new Label
            {
                Content = "ReadTextFile",
                FontSize = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            AddControlToNode(label);
        }


        public override void Calculate()
        {
            string e = Path.GetExtension((string)InputPorts[0].Data);
            if (e == ".txt")
            {
                try
                {
                    OutputPorts[0].Data = File.ReadAllText((string)InputPorts[0].Data);
                }
                catch (Exception ex)
                { }
            }
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

        }

        public override Node Clone()
        {
            return new ReadTextFileNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}