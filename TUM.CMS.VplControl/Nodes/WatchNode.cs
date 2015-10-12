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
            AddInputPortToNode("Object", typeof (object),true);

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
                Content = textBlock
                //IsHitTestVisible = false
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

            if (InputPorts[0].Data == null)
                textBlock.Text = "null";
            else
            {
                Type t;

                var type = InputPorts[0].Data as Type;

                if (type != null)
                {
                    t = type;

                    if (t.IsEnum)
                    {
                        textBlock.Text = "Enum " + t.Name + " {";

                        var counter = 0;
                        foreach (var o in Enum.GetValues(t))
                        {
                            textBlock.Text += o.ToString();

                            if (counter < Enum.GetValues(t).Length - 1)
                                textBlock.Text += ",";

                            counter++;
                        }
                        textBlock.Text += "}";
                    }
                    else
                        textBlock.Text = t.Name + " : Type";
                }
                else
                {
                    t = InputPorts[0].Data.GetType();

                    if (t.IsGenericType)
                    {
                        var collection = InputPorts[0].Data as ICollection;
                        if (collection == null) return;
                        var obj = collection;

                        textBlock.Text = CollectionToString(obj, 1);
                    }
                    else
                        textBlock.Text = InputPorts[0].Data + " : " + t.Name;
                }
            }
        }

        private string CollectionToString(ICollection coll, int depth)
        {
            var tempLine = "";

            for (var i = 0; i < depth - 1; i++)
                tempLine += "  ";

            tempLine = "List" + Environment.NewLine;
            var counter = 0;

            foreach (var item in coll)
            {
                for (var i = 0; i < depth; i++)
                    tempLine += "  ";

                tempLine += "[" + counter + "] ";

                if (item == null)
                {
                    tempLine += "null";

                    if (depth != 1 || counter != coll.Count - 1)
                        tempLine += Environment.NewLine;
                }
                else
                {
                    if (item.GetType().IsGenericType)
                    {
                        var collection = item as ICollection;
                        if (collection == null) return "";
                        var obj = collection;

                        tempLine += CollectionToString(obj, depth + 1);
                    }
                    else
                    {
                        tempLine += item + " : " + item.GetType().Name;

                        if (depth != 1 || counter != coll.Count - 1)
                            tempLine += Environment.NewLine;
                    }
                }

                counter++;
            }
            return tempLine;
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