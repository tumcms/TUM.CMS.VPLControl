using System.ComponentModel.Composition;
using System.Windows;

namespace TUM.CMS.VplControl.Nodes
{
    internal class ViewNode : Node
    {

        public ViewNode(Core.VplControl hostCanvas) : base(hostCanvas)
        {
            AddInputPortToNode("Test 123", typeof (object));

            QuestButton.Visibility = Visibility.Visible;
            BinButton.Visibility = Visibility.Visible;

            TopComment.Visibility = Visibility.Visible;
            BottomComment.Visibility = Visibility.Visible;

            // LabelCaption.Visibility = Visibility.Visible;
        }

        public override void Calculate()
        {
        }

        public override Node Clone()
        {
            return new TestNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}