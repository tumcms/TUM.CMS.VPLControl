using System.Windows.Controls;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class OperatorTestNode : OperatorNode
    {
        // DataController for the connection to the BimPlus Framework
        private DataController _controller;

        public OperatorTestNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            AddInputPortToNode("Project", typeof (object));
            AddOutputPortToNode("Issues", typeof (object));
            DataContext = this;

            var content = new Label
            {
                Height = 200,
                Width = 200,
                Content = "Hallo"
            };

            AddControlToNode(content);

            // Border.CornerRadius = new CornerRadius(content.Height);
            // Border.Width = content.Width;
        }

        public override void Calculate()
        {
            var test = true;
            base.Calculate();
        }
    }
}