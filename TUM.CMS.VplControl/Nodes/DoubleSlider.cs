using System;
using System.Dynamic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace TUM.CMS.VplControl.Nodes
{

    public class SliderExpanderDouble : Expander
    {
        public static readonly DependencyProperty SliderValueProperty =
            DependencyProperty.Register("SliderValue",
                typeof(double), typeof(SliderExpanderDouble)); // optionally metadata for defaults etc.

        public double SliderValue
        {
            get { return (double) GetValue(SliderValueProperty); }
            set { SetValue(SliderValueProperty, value); }
        }


        public static readonly DependencyProperty SliderMinProperty =
            DependencyProperty.Register("SliderMin",
                typeof(double), typeof(SliderExpanderDouble)); // optionally metadata for defaults etc.

        public double SliderMin
        {
            get { return (double) GetValue(SliderMinProperty); }
            set { SetValue(SliderMinProperty, value); }
        }

        public static readonly DependencyProperty SliderMaxProperty =
            DependencyProperty.Register("SliderMax",
                typeof(double), typeof(SliderExpanderDouble)); // optionally metadata for defaults etc.

        public double SliderMax
        {
            get { return (double) GetValue(SliderMaxProperty); }
            set { SetValue(SliderMaxProperty, value); }
        }

        public static readonly DependencyProperty SliderStepProperty =
    DependencyProperty.Register("SliderStep",
        typeof(double), typeof(SliderExpanderDouble)); // optionally metadata for defaults etc.

        public double SliderStep
        {
            get { return (double)GetValue(SliderStepProperty); }
            set { SetValue(SliderStepProperty, value); }
        }
    }


    public class DoubleSlider : Node
    {
        public DoubleSlider(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddOutputPortToNode("Number", typeof(double));

            SliderExpanderDouble expander = new SliderExpanderDouble
            {
                Style = hostCanvas.FindResource("ExpanderSliderStyleDouble") as Style,
                SliderValue = 5,
                SliderMax = 10,
                SliderMin = 2,
                SliderStep = 0.01
            };

            var b2 = new Binding("Data")
            {
                Mode = BindingMode.OneWayToSource,
                Source = OutputPorts[0]
            };
            expander.SetBinding(SliderExpanderDouble.SliderValueProperty, b2);

            
            AddControlToNode(expander);
        }
    
        public override void Calculate()
        {
          
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var expander = ControlElements[0] as SliderExpanderDouble;
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

            var expander = ControlElements[0] as SliderExpanderDouble;
            if (expander == null) return;

            var attribute = xmlReader.GetAttribute("SliderMax");
            if (attribute != null)
                expander.SliderMax = Convert.ToDouble(attribute.Replace(".", ","));

            var attribute1 = xmlReader.GetAttribute("SliderMin");
            if (attribute1 != null)
                expander.SliderMin  = Convert.ToDouble(attribute1.Replace(".", ","));

            var attribute2 = xmlReader.GetAttribute("SliderValue");
            if (attribute2 != null)
                expander.SliderValue = Convert.ToDouble(attribute2.Replace(".", ","));

            var attribute3 = xmlReader.GetAttribute("SliderStep");
            if (attribute3 != null)
                expander.SliderStep = Convert.ToDouble(attribute3.Replace(".", ","));

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