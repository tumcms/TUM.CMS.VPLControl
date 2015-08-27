using System.Windows;
using System.Windows.Media;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus
{ 
    public abstract class OperatorNode : Node
    {
        protected OperatorNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            /*
            var myRotateTransform = new RotateTransform() {Angle = 45};
            var myTransformGroup = new TransformGroup(); 
            myTransformGroup.Children.Add(myRotateTransform);
            */

            if (Border == null) return;

            Border.BorderBrush = Brushes.DarkRed;
            Border.BorderThickness = new Thickness(2);
            Border.CornerRadius = new CornerRadius(0);
        }

        public override void Calculate()
        {
            // throw new NotImplementedException();
        }

        public override Node Clone()
        {
            // throw new NotImplementedException();
            return null;
        }
    }
}