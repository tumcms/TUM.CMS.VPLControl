using System.Windows;

namespace TUM.CMS.VplControl.BimPlus.Controls
{
    /// <summary>
    ///     Interaction logic for RelationNodeControl.xaml
    /// </summary>
    public partial class RelationNodeControl
    {
        public RelationNodeControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ContractButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContractButton.Visibility = Visibility.Collapsed;
            ExpandButton.Visibility = Visibility.Visible;
            ListBox.Visibility = Visibility.Collapsed;
        }

        private void ExpandButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContractButton.Visibility = Visibility.Visible;
            ExpandButton.Visibility = Visibility.Collapsed;
            ListBox.Visibility = Visibility.Visible;
        }
    }
}