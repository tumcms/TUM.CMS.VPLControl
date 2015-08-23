using System;
using System.Windows.Controls;
using TUM.CMS.VplControl.BimPlus.Controls;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class WebBrowserNode : Node
    {
        // DataController
        private DataController _controller;

        public WebBrowserNode(VplControl hostCanvas)
            : base(hostCanvas)
        {
            DataContext = this;

            /*
            Frame frame = new Frame();
            frame.Navigate(new Uri("http://bbc.co.uk"));
            */

            var control = new WebBrowserControl();

            control.WebBrowser.Navigate(new Uri("http://portal-stage.bimplus.net/?embedded=1"));

            var pr = new ContentPresenter
            {
                Content = control,
                Width = 1000,
                Height = 1000
            };
            AddControlToNode(pr);
        }

        public override void Calculate()
        {
        }

        public override Node Clone()
        {
            return new WebPortalNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}