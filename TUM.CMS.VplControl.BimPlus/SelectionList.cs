using System;
using System.Collections.Generic;
using BimPlus.Explorer.Contract.Services.Selection;

namespace TUM.CMS.VplControl.BimPlus
{
    public class SelectionList : ISelectionList
    {
        public SelectionList()
        {
            MSelection = new List<Guid>();
        }
        public string Name { get; set; }
        public ISelectionManager SelectionManager { get;set; }
        protected List<Guid> MSelection;
        public List<Guid> GetSelection()
        {
            return MSelection;
        }
        public void Select(Guid id, bool select, object source = null)
        {
            if (select)
            {
                MSelection.Add(id);
                RaiseSelectionChangedEvent(new List<Guid>()
                {
                    id
                }, null,false, source);
            }
            else
            {
                MSelection.Remove(id);
                RaiseSelectionChangedEvent(null,new List<Guid>()
                {
                    id
                }, false,source);
            }
        }

        public void Select(List<Guid> ids, bool select, object source = null)
        {
            if (select)
            {
                MSelection.AddRange(ids);
                RaiseSelectionChangedEvent(ids, null, false, source);
            }
            else
            {
                foreach(var id in ids)
                {
                    MSelection.Remove(id);
                }
                RaiseSelectionChangedEvent(null, ids, false, source);
            }
        }

        public void Clear(object source = null)
        {
            new List<Guid>(MSelection);
            RaiseSelectionChangedEvent(null, null,true, source);
            MSelection.Clear();
        }

        protected void RaiseSelectionChangedEvent(List<Guid> added, List<Guid> removed, bool cleared, object source)
        {
            // Raise event for this list only
            if (SelectionChanged != null)
            {
                SelectionChanged(null, new SelectionChangedEventArgs()
                {
                    Added = added,
                    Removed = removed,
                    Source = source,
                    Cleared = cleared,
                    SelectionList = this
                });
            }

            // Raise global event
            var selectionManager = SelectionManager as SelectionManager;
            if (selectionManager != null)
                selectionManager.RaiseSelectionChangedEvent(this, added, removed, cleared, source);
        }

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
    }
}
