using System.Collections.ObjectModel;
using BimPlus.IntegrationFramework.Contract.Model;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{

    public class IssueContainerNode : Node
    {
        private readonly DataController _controller;

        private ObservableCollection<Issue> _issues;  
        public IssueContainerNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            AddInputPortToNode("Project", typeof(object));
            AddOutputPortToNode("Issues", typeof(object));
            DataContext = this;
        }

        public override void Calculate()
        {
            if (InputPorts[0] == null) return;

            if (InputPorts[0].Data.GetType() != typeof (Project)) return;

            var project = InputPorts[0].Data as Project;
            if (project == null) return;

            _issues = _controller.IntBase.APICore.GetIssues(project.Id);
            OutputPorts[0].Data = _issues;
        }

        public override Node Clone()
        {
            return new IssueContainerNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}