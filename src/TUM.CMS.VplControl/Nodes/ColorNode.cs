using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TUM.CMS.VplControl.Controls;
using TUM.CMS.VplControl.Core;
using Xceed.Wpf.Toolkit;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace TUM.CMS.VplControl.Nodes
{
    internal class ColorNode : Node
    {
        private readonly ColorCanvas cc;
        private readonly ExpanderColor expander;

        /// <summary>
        /// This is a color node, where you can pick colors from a color panel.
        /// </summary>
        /// <param name="hostCanvas"></param>
        public ColorNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddOutputPortToNode("Color", typeof (Color));

            expander = new ExpanderColor
            {
                Style = hostCanvas.FindResource("ExpanderColorStyle") as Style
            };

            cc = new ColorCanvas
            {
                Background = Brushes.White,
                BorderBrush = Brushes.White
            };


            expander.Content = cc;
            cc.SelectedColorChanged += cc_SelectedColorChanged;

            cc.SelectedColor = Colors.LightGray;

            AddControlToNode(expander);
        }

        private void cc_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (cc.SelectedColor != null)
            {
                expander.Color = (Color) cc.SelectedColor;
                expander.Brush = new SolidColorBrush((Color) cc.SelectedColor);
                OutputPorts[0].Data = (Color) cc.SelectedColor;
            }
        }

        public override void Calculate()
        {
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var color = new Color();
            if (cc.SelectedColor != null)
                color = cc.SelectedColor.Value;

            //Here we convert 
            var drawingColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

            xmlWriter.WriteStartAttribute("Color");
            xmlWriter.WriteValue(ColorTranslator.ToHtml(drawingColor));
            xmlWriter.WriteEndAttribute();
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            var value = xmlReader.GetAttribute("Color");
            if (value != null)
            {
                var drawingColor = ColorTranslator.FromHtml(value);
                cc.SelectedColor = Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
            }
        }

        public override Node Clone()
        {
            return new ColorNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}