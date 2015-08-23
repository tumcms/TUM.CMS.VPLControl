using System.Windows;
using System.Windows.Controls;
using BimPlus.IntegrationFramework.ViewModels;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class WebProjectSelectionNode : Node
    {
        // DataController
        private readonly DataController _controller;

        public WebProjectSelectionNode(VplControl hostCanvas)
            : base(hostCanvas)
        {
            DataContext = this;

            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;
            
            QuestButton.Visibility = Visibility.Visible;
            BinButton.Visibility = Visibility.Visible;

            var vm = new ProjectSelectionViewModel(_controller.DataContainer)
            {
                Visibility = Visibility.Visible
            };

            // vm.RefreshCommand.Execute();

            var pr = new ContentPresenter
            {
                Content = vm,
                Width = 600,
                Height = 450
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