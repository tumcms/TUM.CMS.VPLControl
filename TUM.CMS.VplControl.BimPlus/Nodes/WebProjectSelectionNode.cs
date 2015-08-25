using System.Windows;
using System.Windows.Controls;
using BimPlus.IntegrationFramework.WebControls.ViewModels;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class WebProjectSelectionNode : Node
    {
        // DataController
        private readonly DataController _controller;

        public WebProjectSelectionNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            DataContext = this;

            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            IsResizeable = true;

            var vm = new ProjectSelectionViewModel(_controller.DataContainer)
            {
                Visibility = Visibility.Visible
            };

            vm.RefreshCommand.Execute();

            var pr = new ContentPresenter
            {
                Content = vm,
                MinWidth = 600,
                MinHeight = 600
            };

            AddControlToNode(pr);
        }

        public override void Calculate()
        {
        }

        public override Node Clone()
        {
            return new WebProjectSelectionNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}