using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace TUM.CMS.VplControl.BimPlus.Controls
{
    public partial class WebBrowserControl
    {
        public WebBrowserControl()
        {
            InitializeComponent();
            WebBrowser.Navigate("http://www.wpf-tutorial.com");
        }

        private void txtUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            try
            {
                WebBrowser.Navigate(txtUrl.Text);
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private void wbSample_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            txtUrl.Text = e.Uri.OriginalString;
        }

        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((WebBrowser != null) && (WebBrowser.CanGoBack));
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WebBrowser.GoBack();
        }

        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((WebBrowser != null) && (WebBrowser.CanGoForward));
        }

        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WebBrowser.GoForward();
        }

        private void GoToPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void GoToPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WebBrowser.Navigate(txtUrl.Text);
        }
    }
}