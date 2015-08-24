using System;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Utilities;

namespace TUM.CMS.VplControl.Core
{
    public class NodeGroup : VplElement
    {
        private static int counter;
        private TrulyObservableCollection<Node> childNodes;

        public NodeGroup(VplControl hostCanvas)
            : base(hostCanvas)
        {
            Id = Interlocked.Increment(ref counter);

            Border.MouseDown += HitTestBorder_MouseDown;
            Border.MouseUp += Border_MouseUp;

            CaptionLabel.Width = 200;

            Name = "Name group here...";

            HostCanvas.NodeGroupCollection.Add(this);

            SetZIndex(this, 0);
            SetZIndex(Border, 0);
        }

        public int Id { get; set; }

        public TrulyObservableCollection<Node> ChildNodes
        {
            get { return childNodes; }
            set
            {
                childNodes = value;
                childNodes.CollectionChanged += childNodes_CollectionChanged;
                ObserveAllChildNode();
                CalculateBorder();
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HostCanvas.MouseMode = MouseModes.Nothing;
        }

        private void HitTestBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var node in ChildNodes)
            {
                HostCanvas.MouseMove += node.HostCanvas_MouseMove;
                HostCanvas.MouseUp += node.Node_MouseUp;

                node.OldMousePosition = e.GetPosition(HostCanvas);
                node.IsSelected = true;
                HostCanvas.SelectedNodes.Add(node);
            }

            HostCanvas.MouseMode = MouseModes.GroupSelection;
        }

        public override void binButton_Click(object sender, RoutedEventArgs e)
        {
            Dispose();

            DeleteGroup();
        }

        private void DeleteGroup()
        {
            UnObserveAllChildNode();
            //if(ChildNodes.Count!=0) ChildNodes.Clear();

            HostCanvas.NodeGroupCollection.Remove(this);
            HostCanvas.Children.Remove(Border);
            HostCanvas.Children.Remove(this);

            IsHitTestVisible = false;
        }

        private void childNodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (childNodes.Count <= 1)
                binButton_Click(null, null);
            else
                CalculateBorder();
        }

        private void ObserveAllChildNode()
        {
            foreach (var node in ChildNodes)
                node.DeletedInNodeCollection += node_DeletedInNodeCollection;
        }

        private void UnObserveAllChildNode()
        {
            foreach (var node in ChildNodes)
                node.DeletedInNodeCollection -= node_DeletedInNodeCollection;
        }

        private void node_DeletedInNodeCollection(object sender, EventArgs e)
        {
            var node = sender as Node;
            if (node == null) return;

            ChildNodes.Remove(node);
        }

        private new void CalculateBorder()
        {
            if (ChildNodes.Count == 0) return;
            if (Border == null) return;
            if (HitTestBorder == null) return;

            var minTop = double.MaxValue;
            var maxTop = double.MinValue;
            var minLeft = double.MaxValue;
            var maxLeft = double.MinValue;


            foreach (var node in ChildNodes)
            {
                if (node.Left < minLeft) minLeft = node.Left;
                if (node.Top < minTop) minTop = node.Top;

                if (node.Left + node.ActualWidth > maxLeft) maxLeft = node.Left + node.ActualWidth;
                if (node.Top + node.ActualHeight > maxTop) maxTop = node.Top + node.ActualHeight;
            }

            double offset = 30;

            minLeft -= offset;
            minTop -= offset;
            maxLeft += offset;
            maxTop += offset;

            Border.Width = maxLeft - minLeft;
            Border.Height = maxTop - minTop;
            Canvas.SetLeft(Border, minLeft);
            Canvas.SetTop(Border, minTop);

            HitTestBorder.Width = maxLeft - minLeft;
            HitTestBorder.Height = maxTop - minTop + 30;
            Canvas.SetLeft(HitTestBorder, minLeft);
            Canvas.SetTop(HitTestBorder, minTop - 30);

            OnPropertyChanged("BorderSize");
        }
    }
}