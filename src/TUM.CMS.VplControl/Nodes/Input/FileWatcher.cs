using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Win32;
using TUM.CMS.VplControl.Core;

namespace TUM.CMS.VplControl.Nodes.Input
{
    public class FileWatcherNode : Node
    {
        private readonly TextBlock textBlock;
        private FileSystemWatcher watcher;

        public FileWatcherNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            AddOutputPortToNode("String", typeof (string));

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(60, GridUnitType.Pixel)});
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(100, GridUnitType.Auto)});

            textBlock = new TextBlock {IsHitTestVisible = false, VerticalAlignment = VerticalAlignment.Center};

            textBlock.SetValue(ColumnProperty, 1);

            var button = new Button {Content = "Search"};
            button.Click += button_Click;
            button.Width = 50;

            grid.Children.Add(textBlock);
            grid.Children.Add(button);

            AddControlToNode(grid);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false
                //Filter = "vplXML (.vplxml)|*.vplxml"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                textBlock.Text = openFileDialog.FileName;

                if (watcher != null)
                {
                    watcher.Changed -= Watcher_Changed;
                    watcher.EnableRaisingEvents = false;
                }

                watcher = new FileSystemWatcher();
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                watcher.Changed += Watcher_Changed;
                watcher.Path = Path.GetDirectoryName(textBlock.Text);
                watcher.Filter = Path.GetFileName(textBlock.Text);
                watcher.EnableRaisingEvents = true;

                Calculate();
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(Calculate));
        }

        public override void Calculate()
        {
            OutputPorts[0].Data = textBlock.Text;
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            var textBox = ControlElements[0] as TextBox;
            if (textBox == null) return;

            xmlWriter.WriteStartAttribute("Text");
            xmlWriter.WriteValue(textBox.Text);
            xmlWriter.WriteEndAttribute();
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);

            var textBox = ControlElements[0] as TextBox;
            if (textBox == null) return;

            textBox.Text = xmlReader.GetAttribute("Text");
        }

        public override Node Clone()
        {
            return new FileWatcherNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}