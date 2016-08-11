using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TUM.CMS.VplControl.Utilities
{
    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public TrulyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                    ((INotifyPropertyChanged) item).PropertyChanged += ItemPropertyChanged;
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                    ((INotifyPropertyChanged) item).PropertyChanged -= ItemPropertyChanged;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var args =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender,
                    IndexOf((T) sender));
            OnCollectionChanged(args);
        }
    }
}