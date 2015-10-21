using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32.SafeHandles;
using TUM.CMS.VplControl.Annotations;
using TUM.CMS.VplControl.Utilities;

namespace TUM.CMS.VplControl.Core
{
    /// <summary>
    ///     The superclass for all node types within VplControl.
    /// </summary>
    public abstract class VplElement : Grid, INotifyPropertyChanged, IDisposable
    {
        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public CheckBox AutoCheckBox;
        public Button BinButton;
        public Label CaptionLabel;
        private bool disposed;
        public Border HitTestBorder;
        public Button QuestButton;
        public Button ResizeButton;

        /// <summary>
        ///     Initializes a new instance of VplElement class.
        /// </summary>
        /// <param name="hostCanvas">The host VplControl in which the VplElement will be rendered.</param>
        protected VplElement(VplControl hostCanvas)
        {
            HostCanvas = hostCanvas;
            // ----------------------------------------------------------------------------------------------------------------------
            // Border
            // ----------------------------------------------------------------------------------------------------------------------
            Border = new Border
            {
                Child = this,
                Style = FindResource("VplElementBorderStyle") as Style
            };
            DependencyPropertyDescriptor.FromProperty(IsSelectedProperty, typeof (VplElement))
                .AddValueChanged(this, OnSelectionChanged);
            HostCanvas.Children.Add(Border);

            // ----------------------------------------------------------------------------------------------------------------------
            // HitTestBorder
            // ----------------------------------------------------------------------------------------------------------------------
            HitTestBorder = new Border {Style = FindResource("HitTestBorderStyle") as Style};
            HitTestBorder.MouseEnter += HitTestBorder_MouseEnter;
            HitTestBorder.MouseLeave += HitTestBorder_MouseLeave;

            HostCanvas.Children.Add(HitTestBorder);


            // ----------------------------------------------------------------------------------------------------------------------
            // Buttons
            // ----------------------------------------------------------------------------------------------------------------------
            if (GetType() == typeof (SelectionNode)) return;

            CaptionLabel = new NodeCaptionLabel(this);
            QuestButton = new NodeQuestionButton(this);
            ResizeButton = new NodeResizeButton(this);
            BinButton = new NodeBinButton(this);
            AutoCheckBox = new NodeAutoCheckBox(this);

            CaptionLabel.Width = 80;

            BinButton.Click += binButton_Click;
            
            BinButton.Visibility = Visibility.Collapsed;
            QuestButton.Visibility = Visibility.Collapsed;
            ResizeButton.Visibility = Visibility.Collapsed;
            AutoCheckBox.Visibility = Visibility.Collapsed;
        }

  

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void binButton_Click(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        private void HitTestBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (GetType() == typeof (SelectionNode)) return;

            if (VisualTreeHelper.HitTest(BinButton, e.GetPosition(BinButton)) != null)
                return;
            if (VisualTreeHelper.HitTest(ResizeButton, e.GetPosition(ResizeButton)) != null)
                return;
            if (VisualTreeHelper.HitTest(CaptionLabel, e.GetPosition(CaptionLabel)) != null)
                return;
            if (VisualTreeHelper.HitTest(QuestButton, e.GetPosition(QuestButton)) != null)
                return;
            if (VisualTreeHelper.HitTest(AutoCheckBox, e.GetPosition(AutoCheckBox)) != null)
                return;

            BinButton.Visibility = Visibility.Collapsed;
            QuestButton.Visibility = Visibility.Collapsed;
            ResizeButton.Visibility = Visibility.Collapsed;
            AutoCheckBox.Visibility = Visibility.Collapsed;
        }

        private void HitTestBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (GetType() == typeof (SelectionNode)) return;

            BinButton.Visibility = Visibility.Visible;
            QuestButton.Visibility = Visibility.Visible;
            ResizeButton.Visibility = Visibility.Visible;
            AutoCheckBox.Visibility = Visibility.Visible;
        }

        public Point GetPosition()
        {
            return new Point(Left, Top);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();

                HostCanvas.Children.Remove(QuestButton);
                HostCanvas.Children.Remove(BinButton);
                HostCanvas.Children.Remove(ResizeButton);
                HostCanvas.Children.Remove(CaptionLabel);
                HostCanvas.Children.Remove(AutoCheckBox);
                HostCanvas.Children.Remove(HitTestBorder);

                HitTestBorder.MouseEnter -= HitTestBorder_MouseEnter;
                HitTestBorder.MouseLeave -= HitTestBorder_MouseLeave;

                HitTestBorder = null;

                QuestButton = null;
                BinButton = null;
                ResizeButton = null;
                CaptionLabel = null;

                HostCanvas.Children.Remove(this);
            }

            disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties

        /// <summary>
        ///     The host VplControl in which the VplElement is rendered.
        /// </summary>
        public VplControl HostCanvas { get; set; }

        /// <summary>
        ///     The top coordinate of this element with respect to the host canvas.
        /// </summary>
        public double Top
        {
            get { return (double) Border.GetValue(Canvas.TopProperty); }
            set
            {
                Border.SetValue(Canvas.TopProperty, value);

                if (HitTestBorder != null)
                {
                    HitTestBorder.Height = 30;
                    Canvas.SetTop(HitTestBorder, Top - 30);
                }

                HostCanvas.Refresh();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     The left coordinate of this element with respect to the host canvas.
        /// </summary>
        public double Left
        {
            get { return (double) Border.GetValue(Canvas.LeftProperty); }
            set
            {
                Border.SetValue(Canvas.LeftProperty, value);

                if (HitTestBorder != null)
                {
                    HitTestBorder.Width = ActualWidth + 20;
                    Canvas.SetLeft(HitTestBorder, Left - 10);
                }

                HostCanvas.Refresh();
                OnPropertyChanged();
            }
        }


        public new double Width { get; set; }


        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof (string), typeof (VplElement),
                new UIPropertyMetadata(string.Empty));

        public new string Name
        {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }


        public static readonly DependencyProperty BorderProperty =
            DependencyProperty.Register("Border", typeof (Border), typeof (VplElement));

        public Border Border
        {
            get { return (Border) GetValue(BorderProperty); }
            set { SetValue(BorderProperty, value); }
        }


        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof (bool), typeof (VplElement));

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (IsSelected)
                Border.Style = FindResource("VplElementBorderSelectionStyle") as Style;
            else
                Border.Style = FindResource("VplElementBorderStyle") as Style;
        }

        #endregion
    }
}