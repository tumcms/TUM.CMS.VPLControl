using System;
using System.Windows;
using System.Windows.Data;
using System.Xml;
using TUM.CMS.VplControl.Controls;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Input
{
    
    public class IntegerSlider : Node
    {
        public IntegerSlider(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddOutputPortToNode("Number", typeof(int));

            SliderExpanderInteger expander = new SliderExpanderInteger
            {
                Style = hostCanvas.FindResource("ExpanderSliderStyleInteger") as Style,
                SliderValue = 5,
                SliderMax = 10,
                SliderMin = 0,
                SliderStep = 1
            };

            var b2 = new Binding("Data")
            {
                Mode = BindingMode.OneWayToSource,
                Source = OutputPorts[0]
            };
            expander.SetBinding(SliderExpanderInteger.SliderValueProperty, b2);

            Name = "Integer slider";

            AddControlToNode(expander);
        }
    
        public override void Calculate()
        {
          
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var expander = ControlElements[0] as SliderExpanderInteger;
            if (expander == null) return;

            xmlWriter.WriteStartAttribute("SliderMax");
            xmlWriter.WriteValue(expander.SliderMax);
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("SliderMin");
            xmlWriter.WriteValue(expander.SliderMin);
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("SliderValue");
            xmlWriter.WriteValue(expander.SliderValue);
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("SliderStep");
            xmlWriter.WriteValue(expander.SliderStep);
            xmlWriter.WriteEndAttribute();

            xmlWriter.WriteStartAttribute("IsExpanded");
            xmlWriter.WriteValue(expander.IsExpanded);
            xmlWriter.WriteEndAttribute();
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            var expander = ControlElements[0] as SliderExpanderInteger;
            if (expander == null) return;

            var attribute = xmlReader.GetAttribute("SliderMax");
            if (attribute != null)
                expander.SliderMax = Convert.ToInt32(attribute.Replace(".", ","));

            var attribute1 = xmlReader.GetAttribute("SliderMin");
            if (attribute1 != null)
                expander.SliderMin = Convert.ToInt32(attribute1.Replace(".", ","));

            var attribute2 = xmlReader.GetAttribute("SliderValue");
            if (attribute2 != null)
                expander.SliderValue = Convert.ToInt32(attribute2.Replace(".", ","));

            var attribute3 = xmlReader.GetAttribute("SliderStep");
            if (attribute3 != null)
                expander.SliderStep = Convert.ToInt32(attribute3.Replace(".", ","));

            var attribute4 = xmlReader.GetAttribute("IsExpanded");
            if (attribute4 != null)
                expander.IsExpanded = Convert.ToBoolean(attribute4.Replace(".", ","));
        }

        public override Node Clone()
        {
            return new DoubleSlider(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}