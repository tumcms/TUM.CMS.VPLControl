using System;
using System.Windows;
using System.Windows.Forms;

namespace TUM.CMS.VPL.Scripting
{
    /// <summary>
    ///     Interaction logic for AddAssemblyDialog.xaml
    /// </summary>
    public partial class AssemblyDialog
    {
        private readonly ScriptFile scriptFile;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddAssemblyDialog" /> class.
        /// </summary>
        public AssemblyDialog(ScriptFile file)
        {
            InitializeComponent();
            scriptFile = file;
            ReferencedAssembliedListBox.ItemsSource = scriptFile.ReferencedAssemblies;
        }

        /// <summary>
        ///     Called when user wants to confirm.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnCmdOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        ///     Called when user wants to cancel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void OnCmdCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dia = new OpenFileDialog
            {
                Filter = @"dll files (*.dll)|*.dll|All files (*.*)|*.*",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (dia.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            // ReferencedAssembliedListBox.Items.Add(dia.FileName);
            if (!scriptFile.ReferencedAssemblies.Contains(dia.FileName))
                scriptFile.ReferencedAssemblies.Add(dia.FileName);
        }
    }
}