using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes
{
    internal class TestNode : Node
    {
        public TestNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddControlToNode(new TextBox());
            AddControlToNode(new Button {Content = "test", Width = 120});
            AddControlToNode(new CheckBox {Content = "test"});
            AddControlToNode(new ComboBox {Items = {"Test1", "Test2", "Test3"}});
            AddControlToNode(new Slider {Value = 5});

            AddInputPortToNode("Test 123", typeof (double));
            AddInputPortToNode("Test 12345", typeof (string));
            AddInputPortToNode("Test 123456789", typeof (int));
            AddInputPortToNode("Test", typeof (bool));
            AddInputPortToNode("Test", typeof (List<string>));

            AddOutputPortToNode("Test 123", typeof (double));
            AddOutputPortToNode("Test 12345", typeof (string));
            AddOutputPortToNode("Test 12345", typeof (int));
            AddOutputPortToNode("Test 12345", typeof (bool));
            AddOutputPortToNode("Test 12345", typeof (List<string>));


            //TopComment.Visibility = Visibility.Visible;
            //BottomComment.Visibility = Visibility.Visible;

            IsResizeable = true;

            Name = "Test node";
        }

        public override void Calculate()
        {

                string[] test=null;

                string test2 = test[2];

       }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var textBox = ControlElements[0] as TextBox;
            if (textBox != null)
            {
                xmlWriter.WriteStartAttribute("Text");
                xmlWriter.WriteValue(textBox.Text);
                xmlWriter.WriteEndAttribute();
            }

            var checkBox = ControlElements[2] as CheckBox;
            if (checkBox != null)
            {
                xmlWriter.WriteStartAttribute("CheckBoxIsChecked");
                if (checkBox.IsChecked != null) xmlWriter.WriteValue(checkBox.IsChecked);
                xmlWriter.WriteEndAttribute();
            }

            var comboBox = ControlElements[3] as ComboBox;
            if (comboBox != null)
            {
                xmlWriter.WriteStartAttribute("SelectedIndex");
                xmlWriter.WriteValue(comboBox.SelectedIndex);
                xmlWriter.WriteEndAttribute();
            }

            var slider = ControlElements[4] as Slider;
            if (slider != null)
            {
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
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            var value = xmlReader.GetAttribute("Text");

            var textBox = ControlElements[0] as TextBox;
            if (textBox != null)
                textBox.Text = value;

            var checkBox = ControlElements[2] as CheckBox;
            if (checkBox != null)
                checkBox.IsChecked = Convert.ToBoolean(xmlReader.GetAttribute("CheckBoxIsChecked"));


            value = xmlReader.GetAttribute("SelectedIndex");
            var index = Convert.ToInt32(value);

            var comboBox = ControlElements[3] as ComboBox;
            if (comboBox != null)
                comboBox.SelectedIndex = index;

            var slider = ControlElements[4] as Slider;
            if (slider != null)
            {
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
        }

        public override Node Clone()
        {
            return new TestNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}