using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace TUM.CMS.VplControl.Nodes
{
    public class SliderNode : Node
    {
        public SliderNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddOutputPortToNode("Number", typeof (double));

            var slider = new Slider {Value = 0.5, Minimum = 0, Maximum = 1, IsHitTestVisible = true, Width = 135};

            AddControlToNode(slider);
            OutputPorts[0].Data = slider.Value;
            slider.ValueChanged += slider_ValueChanged;
        }

        // Solve with Binding would be a more nice way
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Calculate();
        }

        public override void Calculate()
        {
            var slider = ControlElements[0] as Slider;
            if (slider == null) return;

            OutputPorts[0].Data = Math.Round(slider.Value, 3);
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var slider = ControlElements[0] as Slider;
            if (slider == null) return;

            xmlWriter.WriteStartAttribute("SliderMax");
            xmlWriter.WriteValue(slider.Maximum);
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("SliderMin");
            xmlWriter.WriteValue(slider.Minimum);
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("SliderValue");
            xmlWriter.WriteValue(slider.Value);
            xmlWriter.WriteEndAttribute();
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            var slider = ControlElements[0] as Slider;
            if (slider == null) return;

            var attribute = xmlReader.GetAttribute("SliderMax");
            if (attribute != null)
                slider.Maximum = Convert.ToDouble(attribute.Replace(".", ","));

            var attribute1 = xmlReader.GetAttribute("SliderMin");
            if (attribute1 != null)
                slider.Minimum = Convert.ToDouble(attribute1.Replace(".", ","));

            var attribute2 = xmlReader.GetAttribute("SliderValue");
            if (attribute2 != null)
                slider.Value = Convert.ToDouble(attribute2.Replace(".", ","));
        }

        public override Node Clone()
        {
            return new SliderNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}