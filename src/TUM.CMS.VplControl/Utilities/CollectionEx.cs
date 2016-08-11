using System.Collections.Generic;
using System.ComponentModel;

namespace TUM.CMS.VplControl.Utilities
{
    public static class CollectionEx
    {
        public static TrulyObservableCollection<T> ToTrulyObservableCollection<T>(this IEnumerable<T> enumerableList)
            where T : INotifyPropertyChanged
        {
            if (enumerableList != null)
            {
                // Create an emtpy observable collection object
                var observableCollection = new TrulyObservableCollection<T>();

                // Loop through all the records and add to observable collection object
                foreach (var item in enumerableList)
                    observableCollection.Add(item);

                // Return the populated observable collection
                return observableCollection;
            }
            return null;
        }
    }
}