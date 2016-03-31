using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace TUM.CMS.VPL.Scripting
{
    public class NonFilterStrategy : IFilterStrategy
    {
        /// <summary>
        ///     Filters the specified completion items.
        /// </summary>
        /// <param name="completionItems">The completion items.</param>
        public IEnumerable<ICompletionItem> Filter(IEnumerable<ICompletionItem> completionItems)
        {
            return completionItems;
        }
    }
}