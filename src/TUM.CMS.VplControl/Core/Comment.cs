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
                CanContentScroll = true,
                Background = Brushes.Transparent
            };

            var textBlock = new TextBlock
            {
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 12
            };
            scrollViewer.Content = textBlock;

            CornerRadius= new CornerRadius(5);
            Child = scrollViewer;
            BorderThickness = new Thickness(2);
            Background = Application.Current.Resources["BrushBlue"] as Brush;

            var bindingTextToTextBlock = new Binding("Text")
            {
                Source = this,
                Mode = BindingMode.OneWay
            };
            textBlock.SetBinding(TextBlock.TextProperty, bindingTextToTextBlock);

            HostNode.HostCanvas.Children.Add(this);

            HostNode_PropertyChanged(null, null);
            HostNode.PropertyChanged+= HostNode_PropertyChanged;
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

        /// <summary>
        ///     The host VplControl in which the VplElement is rendered.
        /// </summary>
        public VplControl HostCanvas { get; set; }
        public Node HostNode { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void HostNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (ExpandSide)
            {
                case CommentExpandSides.Top:
                    SetValue(Canvas.TopProperty, HostNode.Top- 125);
                    SetValue(Canvas.LeftProperty, HostNode.Left+5);
                    Width = HostNode.ActualWidth;
                    Height = 100;
                    break;
                case CommentExpandSides.Bottom:
                    SetValue(Canvas.TopProperty, HostNode.Top + HostNode.ActualHeight + 25);
                    SetValue(Canvas.LeftProperty, HostNode.Left+5);
                    Width = HostNode.ActualWidth;
                    Height = 100;
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