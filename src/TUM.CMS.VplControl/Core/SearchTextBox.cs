using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TUM.CMS.VplControl.Core
{
    public class SearchTextBox : TextBox
    {
        public static DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
                "LabelText",
                typeof (string),
                typeof (SearchTextBox));

        public static DependencyProperty LabelTextColorProperty =
            DependencyProperty.Register(
                "LabelTextColor",
                typeof (Brush),
                typeof (SearchTextBox));

        private static readonly DependencyPropertyKey HasTextPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "HasText",
                typeof (bool),
                typeof (SearchTextBox),
                new PropertyMetadata());

        public static DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey IsMouseLeftButtonDownPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsMouseLeftButtonDown",
                typeof (bool),
                typeof (SearchTextBox),
                new PropertyMetadata());

        public static DependencyProperty IsMouseLeftButtonDownProperty =
            IsMouseLeftButtonDownPropertyKey.DependencyProperty;

        public static readonly RoutedEvent SearchEvent =
            EventManager.RegisterRoutedEvent(
                "Search",
                RoutingStrategy.Bubble,
                typeof (RoutedEventHandler),
                typeof (SearchTextBox));

        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (SearchTextBox),
                new FrameworkPropertyMetadata(typeof (SearchTextBox)));
        }

        public string LabelText
        {
            get { return (string) GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public Brush LabelTextColor
        {
            get { return (Brush) GetValue(LabelTextColorProperty); }
            set { SetValue(LabelTextColorProperty, value); }
        }

        public bool HasText
        {
            get { return (bool) GetValue(HasTextProperty); }
            private set { SetValue(HasTextPropertyKey, value); }
        }

        public bool IsMouseLeftButtonDown
        {
            get { return (bool) GetValue(IsMouseLeftButtonDownProperty); }
            private set { SetValue(IsMouseLeftButtonDownPropertyKey, value); }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            RaiseSearchEvent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            var iconBorder = GetTemplateChild("PreviousItemBorder") as Border;
            if (iconBorder != null)
                iconBorder.MouseLeftButtonDown += IconBorder_MouseLeftButtonDown;

            var searchBorder = GetTemplateChild("SearchItemBorder") as Border;
            if (searchBorder != null)
                searchBorder.MouseLeftButtonDown += IconBorder_MouseLeftButtonDown;

            var deleteBorder = GetTemplateChild("DeleteItemBorder") as Border;
            if (deleteBorder != null)
                deleteBorder.MouseLeftButtonDown += IconBorder_MouseLeftButtonDown;
        }

        private void IconBorder_MouseLeftButtonDown(object obj, MouseButtonEventArgs e)
        {
            var border = obj as Border;
            if (border != null && border.Name == "DeleteItemBorder")
                Text = "";
            else if (border != null && border.Name == "SearchItemBorder")
                Text = "";
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Text = "";
            else if (e.Key == Key.Enter)
                RaiseEvent(new SearchEventArgs(SearchEvent) {Keyword = Text});
        }

        private void RaiseSearchEvent()
        {
            RaiseEvent(new SearchEventArgs(SearchEvent) {Keyword = Text});
        }

        public event RoutedEventHandler OnSearch
        {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (!HasText)
                LabelText = "Search";
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (!HasText)
                LabelText = "";
        }
    }

    public class SearchEventArgs : RoutedEventArgs
    {
        private List<string> sections = new List<string>();

        public SearchEventArgs()
        {
        }

        public SearchEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        public string Keyword { get; set; } = "";
    }
}