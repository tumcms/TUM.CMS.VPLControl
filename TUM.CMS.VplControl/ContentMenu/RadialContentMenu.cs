using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32.SafeHandles;
using RadialMenu.Controls;
using TUM.CMS.VplControl.Core;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Utilities;

namespace TUM.CMS.VplControl.ContentMenu
{
    public class RadialContentMenu : RadialMenu.Controls.RadialMenu, IDisposable
    {
        private readonly List<string> alignNames;
        private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        private readonly List<string> menuNames;
        private List<RadialMenuItem> alignMenuItems;
        private bool disposed;
        private List<RadialMenuItem> mainMenuItems;

        public RadialContentMenu(Core.VplControl hostCanvas)
        {
            HostCanvas = hostCanvas;

            CentralItem = FindResource("MyRadialMenuCentralItem") as RadialMenuCentralItem;
            if (CentralItem != null) CentralItem.Click += CentralItem_Click;

            menuNames = new List<string>
            {
                "New",
                "Open",
                "Save",
                "Settings",
                "ZoomToFit",
                "Align",
                "Group",
                "Help"
            };

            InitializeMainMenuItems();

            alignNames = new List<string>
            {
                "Left",
                "HCenter",
                "Right",
                "Top",
                "VCenter",
                "Bottom",
                "ArrangeH",
                "ArrangeV",
                "Back"
            };

            Items = mainMenuItems;


            Panel.SetZIndex(this, 9999999);
        }

        public Core.VplControl HostCanvas { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void InitializeMainMenuItems()
        {
            mainMenuItems = new List<RadialMenuItem>();

            foreach (var name in menuNames)
            {
                var radialMenuitem = FindResource("RadialMenuItem" + name) as RadialMenuItem;
                if (radialMenuitem == null) continue;

                radialMenuitem.Name = "Menu" + name;
                radialMenuitem.Click += radialMenuitem_Click;
                mainMenuItems.Add(radialMenuitem);
            }
        }

        private void InitializeAlignMenuItems()
        {
            alignMenuItems = new List<RadialMenuItem>();

            foreach (var name in alignNames)
            {
                var radialMenuitem = FindResource("RadialMenuItem" + name) as RadialMenuItem;
                if (radialMenuitem == null) continue;

                radialMenuitem.Name = "Align" + name;
                radialMenuitem.Click += radialMenuitem_Click;
                alignMenuItems.Add(radialMenuitem);
            }
        }

        private void radialMenuitem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as RadialMenuItem;

            if (item != null)
            {
                var bBox = new Rect();
                if (item.Name.Contains("Align"))
                    bBox = Node.GetBoundingBoxOfNodes(HostCanvas.SelectedNodes.ToList());

                switch (item.Name)
                {
                    case "MenuNew":
                        HostCanvas.NewFile();
                        break;
                    case "MenuOpen":
                        HostCanvas.OpenFile();
                        break;
                    case "MenuSave":
                        HostCanvas.SaveFile();
                        break;
                    case "MenuSettings":
                        var window = new Settings();
                        window.VplPropertyGrid.SelectedObject = HostCanvas.Theme;
                        window.Show();
                        break;
                    case "MenuZoomToFit":


                        // fit pan
                        bBox = Node.GetBoundingBoxOfNodes(HostCanvas.NodeCollection.ToList());

                        var deltaX = (bBox.Left + bBox.Width/2) - HostCanvas.ActualWidth/2;
                        var deltaY = (bBox.Top + bBox.Height/2) - HostCanvas.ActualHeight/2;

                        foreach (var node in HostCanvas.NodeCollection)
                        {
                            node.Left -= deltaX;
                            node.Top -= deltaY;
                        }

                        HostCanvas.Refresh();

                        // fit scale
                        bBox = Node.GetBoundingBoxOfNodes(HostCanvas.NodeCollection.ToList());

                        var ratioX = bBox.Width/HostCanvas.ActualWidth;
                        var ratioY = bBox.Height/HostCanvas.ActualHeight;
                        var ratio = Math.Max(ratioX, ratioY);
                        //ratio =Math.Ceiling(ratio*10)/10-1;
                        ratio -= 1;

                        if (ratio < 0)
                        {
                            HostCanvas.DoZoomIn(new Point(HostCanvas.ActualWidth/2, HostCanvas.ActualHeight/2),
                                Math.Abs(ratio));
                        }
                        else
                        {
                            HostCanvas.DoZoomOut(new Point(HostCanvas.ActualWidth/2, HostCanvas.ActualHeight/2),
                                Math.Abs(ratio) + 0.3);
                        }

                        HostCanvas.Refresh();

                        // fit pan
                        bBox = Node.GetBoundingBoxOfNodes(HostCanvas.NodeCollection.ToList());

                        deltaX = (bBox.Left + bBox.Width/2) - HostCanvas.ActualWidth/2;
                        deltaY = (bBox.Top + bBox.Height/2) - HostCanvas.ActualHeight/2;

                        foreach (var node in HostCanvas.NodeCollection)
                        {
                            node.Left -= deltaX;
                            node.Top -= deltaY;
                        }
                        HostCanvas.Refresh();

                        break;
                    case "MenuAlign":
                        InitializeAlignMenuItems();
                        Items = alignMenuItems;
                        break;
                    case "MenuGroup":
                        HostCanvas.GroupNodes();
                        break;
                    case "MenuHelp":

                        break;
                    case "AlignLeft":
                        foreach (var node in HostCanvas.SelectedNodes)
                            node.Left = bBox.Left;
                        break;
                    case "AlignHCenter":
                        foreach (var node in HostCanvas.SelectedNodes)
                            node.Left = bBox.Right - bBox.Width/2 - node.ActualWidth/2;
                        break;
                    case "AlignRight":
                        foreach (var node in HostCanvas.SelectedNodes)
                            node.Left = bBox.Right - node.ActualWidth;
                        break;
                    case "AlignTop":
                        foreach (var node in HostCanvas.SelectedNodes)
                            node.Top = bBox.Top;
                        break;
                    case "AlignVCenter":
                        foreach (var node in HostCanvas.SelectedNodes)
                            node.Top = bBox.Bottom - bBox.Height/2 - node.ActualHeight/2;
                        break;
                    case "AlignBottom":
                        foreach (var node in HostCanvas.SelectedNodes)
                            node.Top = bBox.Bottom - node.ActualHeight;
                        break;
                    case "AlignArrangeH":

                        break;
                    case "AlignArrangeV":

                        break;
                    case "AlignBack":
                        Items = mainMenuItems;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (item.Name != "MenuAlign" && item.Name != "AlignBack")
                    Close();
            }
        }

        private void CentralItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Close()
        {
            IsOpen = false;
        }

        // Protected implementation of Dispose pattern. 
        public void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();

                if (mainMenuItems != null)
                {
                    foreach (var item in mainMenuItems)
                        item.Click -= radialMenuitem_Click;
                }

                if (alignMenuItems != null)
                {
                    foreach (var item in alignMenuItems)
                        item.Click -= radialMenuitem_Click;
                }
            }

            disposed = true;
        }
    }
}