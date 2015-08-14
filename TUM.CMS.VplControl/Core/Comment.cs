using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.Core
{
    public sealed class Comment : Border, INotifyPropertyChanged
    {
        private string text;

        public Comment(Node hostNode)
        {
            HostNode = hostNode;

            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                Height = 70,
                CanContentScroll = true
            };

            var textBlock = new TextBlock
            {
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 12
            };

            Child = scrollViewer;
            CornerRadius = new CornerRadius(5);
            scrollViewer.Content = textBlock;


            var bindingTextToTextBlock = new Binding("Text")
            {
                Source = this,
                Mode = BindingMode.OneWay
            };
            textBlock.SetBinding(TextBlock.TextProperty, bindingTextToTextBlock);

            hostNode.SpaceCanvas.Children.Add(this);
        }

        public CommentExpandSides ExpandSide { get; set; }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }

        public Node HostNode { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void HostNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (ExpandSide)
            {
                case CommentExpandSides.Top:
                    Canvas.SetTop(this, -ActualHeight - 10);
                    Canvas.SetLeft(this, -5);
                    Width = HostNode.Border.Width;
                    break;
                case CommentExpandSides.Bottom:
                    Canvas.SetTop(this, HostNode.ActualHeight + 10);
                    Canvas.SetLeft(this, -5);
                    Width = HostNode.Border.Width;
                    break;
                case CommentExpandSides.Left:
                    break;
                case CommentExpandSides.Right:
                    break;
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum CommentExpandSides
    {
        Top,
        Bottom,
        Left,
        Right
    }
}