using System;
using System.Windows;
using System.Windows.Threading;

namespace TUM.CMS.VplControl.Utilities
{
    public static class ExtensionMethods
    {
        private static readonly Action EmptyDelegate = delegate { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}