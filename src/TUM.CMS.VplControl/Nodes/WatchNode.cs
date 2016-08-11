using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes
{
    public class WatchNode : Node
    {
        public WatchNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddInputPortToNode("Object", typeof (object), true);

            var textBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Padding = new Thickness(5),
                IsHitTestVisible = false
            };

            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MinWidth = 120,
                MinHeight = 20,
                MaxWidth = 200,
                MaxHeight = 400,
                CanContentScroll = true,
                Content = textBlock,
               // IsHitTestVisible = false
            };

            AddControlToNode(scrollViewer);
        }


        public override void Calculate()
        {
            foreach (var port in InputPorts)
            {
                //if(port.MultipleConnectionsAllowed)
                //port.CalculateData();
            }

            if (InputPorts[0] == null || ControlElements[0] == null) return;

            var scrollViewer = ControlElements[0] as ScrollViewer;
            if (scrollViewer == null) return;

            var textBlock = scrollViewer.Content as TextBlock;
            if (textBlock == null) return;

            textBlock.Text = Utilities.Utilities.DataToString(InputPorts[0].Data);
        }

       

        public override Node Clone()
        {
            return new WatchNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}