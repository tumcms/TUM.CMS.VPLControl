using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32.SafeHandles;

namespace TUM.CMS.VplControl.Core
{
    public abstract class VplElement : Grid, INotifyPropertyChanged, IDisposable
    {
        public static readonly DependencyProperty nodeGroupNameProperty =
            DependencyProperty.Register("Name", typeof (string), typeof (NodeGroup),
                new UIPropertyMetadata(string.Empty));

        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public CheckBox AutoCheckBox;
        public Button BinButton;
        public Label CaptionLabel;
        private bool disposed;
        public Border HitTestBorder;
        public Button QuestButton;
        public Button ResizeButton;

        protected VplElement(VplControl hostCanvas)
        {
            HostCanvas = hostCanvas;

            Border = new Border
            {
                Style = FindResource("VplElementBorderStyle") as Style,
                Child = this
            };



            // Move to WPF Style
            HitTestBorder = new Border
            {
                Background = Brushes.Transparent,
                IsHitTestVisible = true,
                BorderThickness = new Thickness(1)
            };

            HitTestBorder.MouseEnter += HitTestBorder_MouseEnter;
            HitTestBorder.MouseLeave += HitTestBorder_MouseLeave;

            HostCanvas.Children.Add(HitTestBorder);


            // ----------------------------------------------------------------------------------------------------------------------
            // Buttons
            // ----------------------------------------------------------------------------------------------------------------------

            if (GetType() != typeof (SelectionNode))
            {
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

            HostCanvas.Children.Add(Border);

            Loaded += VplElement_Loaded;
        }

        public new string Name
        {
            get { return (string) GetValue(nodeGroupNameProperty); }
            set { SetValue(nodeGroupNameProperty, value); }
        }

        public double Top
        {
            get { return (double) Border.GetValue(Canvas.TopProperty); }
            set
            {
                Canvas.SetTop(Border, value);
                CalculateBorder();
            }
        }

        public double Left
        {
            get { return (double) Border.GetValue(Canvas.LeftProperty); }
            set
            {
                Canvas.SetLeft(Border, value);
                CalculateBorder();
            }
        }


        public new double Width { get; set; }


        public static readonly DependencyProperty BorderProperty =
            DependencyProperty.Register("Border", typeof(Border), typeof(VplElement));

        public Border Border
        {
            get { return (Border)GetValue(BorderProperty); }
            set { SetValue(BorderProperty, value); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (isSelected)
                    Border.Style = FindResource("VplElementBorderStyleSelection") as Style;
                else
                    Border.Style = FindResource("VplElementBorderStyle") as Style;
            } 
        }


        public VplControl HostCanvas { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;



        private void VplElement_Loaded(object sender, RoutedEventArgs e)
        {
            CalculateBorder();
        }

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

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void CalculateBorder()
        {
            if (HitTestBorder == null) return;

            HitTestBorder.Width = ActualWidth + 10;
            HitTestBorder.Height = ActualHeight;
            Canvas.SetLeft(HitTestBorder, Left);
            Canvas.SetTop(HitTestBorder, Top - 30);

            OnPropertyChanged("BorderSize");
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
    }
}