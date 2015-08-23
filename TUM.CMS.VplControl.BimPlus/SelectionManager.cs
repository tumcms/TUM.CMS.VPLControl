using System;
using System.Collections.Generic;
using System.Windows.Controls;
using BimPlus.Explorer.Contract.Services.Selection;
using SelectionChangedEventArgs = BimPlus.Explorer.Contract.Services.Selection.SelectionChangedEventArgs;

namespace TUM.CMS.VplControl.BimPlus
{
    public class SelectionManager : ISelectionManager
    {
        protected ISelectionList MDefaultSelection;
        protected Dictionary<string, ISelectionList> MSelectionLists;

        public SelectionManager()
        {
            MSelectionLists = new Dictionary<string, ISelectionList>();

            MDefaultSelection = CreateSelectionList("Default");
        }

        public ISelectionList CreateSelectionList(string name)
        {
            var list = new SelectionList
            {
                Name = name,
                SelectionManager = this
            };

            MSelectionLists.Add(name, list);

            return list;
        }

        public bool RemoveSelectionList(string name)
        {
            return MSelectionLists.Remove(name);
        }

        public ISelectionList GetSelectionList(string name)
        {
            ISelectionList list;
            return MSelectionLists.TryGetValue(name, out list) ? list : null;
        }

        public ISelectionList GetDefaultSelectionList()
        {
            return MDefaultSelection;
        }

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        internal void RaiseSelectionChangedEvent(ISelectionList list, List<Guid> added, List<Guid> removed, bool cleared,
            object source)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(null, new SelectionChangedEventArgs
                {
                    Added = added,
                    Removed = removed,
                    Source = source,
                    Cleared = cleared,
                    SelectionList = list
                });
            }
        }
    }
}