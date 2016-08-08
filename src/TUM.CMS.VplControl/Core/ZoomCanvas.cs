using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TUM.CMS.VplControl.Core
{
    public class ZoomCanvas : Canvas
    {
        public readonly ScaleTransform ScaleTransform = new ScaleTransform();
        public readonly TranslateTransform TranslateTransform = new TranslateTransform();
        internal MouseMode mouseMode = MouseMode.Nothing;

        protected Point origin;

        public List<UIElement> SelectedUiElements = new List<UIElement>();
        protected Point start;

        public ZoomCanvas()
        {
            MouseWheel += HandleMouseWheel;
            MouseDown += HandleMouseDown;
            MouseUp += HandleMouseUp;
            MouseMove += HandleMouseMove;
        }

        public void AddChildren(UIElement child)
        {
            Initialize(child);
            Children.Add(child);
        }

        public void Initialize(UIElement element)
        {
            if (element != null)
            {
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(ScaleTransform);
                transformGroup.Children.Add(TranslateTransform);

                element.RenderTransform = transformGroup;
                element.RenderTransformOrigin = new Point(0.0, 0.0);
            }
        }

        public void Reset()
        {
            // reset zoom
            ScaleTransform.ScaleX = 1.0;
            ScaleTransform.ScaleY = 1.0;

            // reset pan
            TranslateTransform.X = ActualWidth/2;
            TranslateTransform.Y = ActualHeight/2;
        }

        #region Events

        protected virtual void HandleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoom = e.Delta > 0 ? .2 : -.2;

            if (!(e.Delta > 0) && (ScaleTransform.ScaleX < .4 || ScaleTransform.ScaleY < .4))
                return;

            foreach (Border child in Children)
            {
                child.UpdateLayout();

                var position = e.GetPosition(child);
                child.RenderTransformOrigin = new Point(position.X/child.ActualWidth, position.Y/child.ActualHeight);
            }

            ScaleTransform.ScaleX += zoom;
            ScaleTransform.ScaleY += zoom;
        }

        protected virtual void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(this);
            origin = new Point(TranslateTransform.X, TranslateTransform.Y);


            foreach (UIElement child in Children)
            {
                if (VisualTreeHelper.HitTest(child, e.GetPosition(child)) != null)
                {
                    SelectedUiElements.Add(child);
                    mouseMode = MouseMode.Selection;
                }
            }

            if (mouseMode != MouseMode.Selection && e.ChangedButton == MouseButton.Middle)
            {
                Cursor = Cursors.Hand;
                mouseMode = MouseMode.Panning;
            }
        }

        protected virtual void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseMode == MouseMode.Panning)
            {
                Cursor = Cursors.Arrow;
            }
            else if (mouseMode == MouseMode.Selection)
            {
                SelectedUiElements.Clear();
            }

            if (e.ChangedButton == MouseButton.Right)
            {
                Reset();
            }

            mouseMode = MouseMode.Nothing;
        }

        protected virtual void HandleMouseMove(object sender, MouseEventArgs e)
        {
            var v = start - e.GetPosition(this);

            if (mouseMode == MouseMode.Panning)
            {
                TranslateTransform.X = origin.X - v.X;
                TranslateTransform.Y = origin.Y - v.Y;
            }
            else if (mouseMode == MouseMode.Selection)
            {
                foreach (var child in SelectedUiElements)
                {
                    SetLeft(child, GetLeft(child) - v.X);
                    SetTop(child, GetTop(child) - v.Y);
                }

                start = e.GetPosition(this);
            }
        }

        #endregion
    }
}