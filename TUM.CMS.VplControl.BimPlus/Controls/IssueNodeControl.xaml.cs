using System;
using System.Windows;

namespace TUM.CMS.VplControl.BimPlus.Controls
{
    /// <summary>
    ///     Interaction logic for IssueNodeControl.xaml
    /// </summary>
    public partial class IssueNodeControl
    {
        public IssueNodeControl()
        {
            InitializeComponent();
        }

        public event EventHandler BaseButtonClicked;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // EventHandler raise
            if (BaseButtonClicked != null)
                BaseButtonClicked(this, new EventArgs());
        }
    }
}