using System;
using System.Dynamic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace TUM.CMS.VplControl.Nodes
{

    public class SliderExpanderInteger : Expander
    {
        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register("SliderValue",
                typeof(int), typeof(SliderExpanderInteger)); // optionally metadata for defaults etc.

        public int SliderValue
        {
            get { return (int)GetValue(SliderValueProperty); }
            set { SetValue(SliderValueProperty, value); }
        }


        public static readonly DependencyProperty SliderMinProperty =
            DependencyProperty.Register("SliderMin",
                typeof(int), typeof(SliderExpanderInteger)); // optionally metadata for defaults etc.

        public int SliderMin
        {
            get { return (int)GetValue(SliderMinProperty); }
            set { SetValue(SliderMinProperty, value); }
        }

        public static readonly DependencyProperty SliderMaxProperty =
            DependencyProperty.Register("SliderMax",
                typeof(int), typeof(SliderExpanderInteger)); // optionally metadata for defaults etc.

        public int SliderMax
        {
            get { return (int)GetValue(SliderMaxProperty); }
            set { SetValue(SliderMaxProperty, value); }
        }

        public static readonly DependencyProperty SliderStepProperty =
    DependencyProperty.Register("SliderStep",
        typeof(int), typeof(SliderExpanderInteger)); // optionally metadata for defaults etc.

        public int SliderStep
        {
            get { return (int)GetValue(SliderStepProperty); }
            set { SetValue(SliderStepProperty, value); }
        }
    }



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