using System.Windows;
using System.Windows.Controls;
using BimPlus.IntegrationFramework.ViewModels;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class WebPortalNode : Node
    {
        // DataController
        private DataController _controller;

        public WebPortalNode(VplControl hostCanvas)
            : base(hostCanvas)
        {
            DataContext = this;

            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            QuestButton.Visibility = Visibility.Visible;
            BinButton.Visibility = Visibility.Visible;

            var vM = new ProjectSelectionViewModel(_controller.DataContainer)
            {
                ProjectSelectionUrl = "http://portal-dev.bimplus.net/#/viewer?cross_token=" +
                                          _controller.DataContainer.RequestCrossToken() + "&team_id=" + _controller.DataContainer.GetCurrentTeam().Id,
                Visibility = Visibility.Visible
            };

            vM.RefreshCommand.Execute();

            var pr = new ContentPresenter
            {
                Content = vM,
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
            return new WebPortalNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}