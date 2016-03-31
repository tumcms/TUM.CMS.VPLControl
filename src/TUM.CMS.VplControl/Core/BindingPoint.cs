using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TUM.CMS.VplControl.Annotations;

namespace TUM.CMS.VplControl.Core
{
    public class BindingPoint : INotifyPropertyChanged
    {
        private Point point;

        public BindingPoint(double x, double y)
        {
            point = new Point(x, y);
        }

        public double X
        {
            get { return point.X; }
            set
            {
                point.X = value;
                OnPropertyChanged();
                OnPropertyChanged("Point");
            }
        }

        public double Y
        {
            get { return point.Y; }
            set
            {
                point.Y = value;
                OnPropertyChanged();
                OnPropertyChanged("Point");
            }
        }

        public Point Point
        {
            get { return point; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}