using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VplControl.Utilities;

namespace TUM.CMS.VplControl.Core
{
    internal class SelectionNode : Node
    {
        public SelectionNode(VplControl hostCanvas) : base(hostCanvas)
        {
            var listBox = new ListBox();

            var typeList = new List<Type>();

            switch (hostCanvas.NodeTypeMode)
            {
                case NodeTypeModes.OnlyInternalTypes:
                    typeList.AddRange(
                        ClassUtility.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TUM.CMS.VplControl.Nodes")
                            .ToList());
                    break;
                case NodeTypeModes.OnlyExternalTypes:
                    typeList.AddRange(hostCanvas.ExternalNodeTypes);
                    break;
                case NodeTypeModes.All:
                    typeList.AddRange(
                        ClassUtility.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "TUM.CMS.VplControl.Nodes")
                            .ToList());
                    typeList.AddRange(hostCanvas.ExternalNodeTypes);
                    break;
            }

            typeList = typeList.OrderBy(x => x.Name).ToList();

            foreach (var type in typeList)
            {
                if (!type.IsAbstract)
                    listBox.Items.Add(type);
                listBox.DisplayMemberPath = "Name";
            }

            listBox.SelectionChanged += listBox_SelectionChanged;
            listBox.MaxHeight = 300;
            listBox.BorderBrush = Brushes.White;

            // Add a autocompletecombobox
            // AutoCompletion with the CustomNodes for the filter ComboBox
            var autoCompleteComboBox = new ComboBox
            {
                ItemsSource = typeList,
                DisplayMemberPath = "Name",
                IsEditable = true,
                IsTextSearchEnabled = true
            };
            autoCompleteComboBox.KeyDown += AutoCompleteComboBoxOnKeyDown;
            // autoCompleteComboBox.Enter
            AddControlToNode(autoCompleteComboBox);

            AddControlToNode(listBox);

            autoCompleteComboBox.Focus();

            Border.MouseLeave += SelectionNode_MouseLeave;
        }

        private void AutoCompleteComboBoxOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key != Key.Return) return;
            var autoCompleteComboBox = sender as ComboBox;
            if (autoCompleteComboBox == null) return;

            var selectedType = autoCompleteComboBox.SelectedItem as Type;
            if (selectedType == null) return;
            var node = (Node) Activator.CreateInstance(selectedType, HostCanvas);
            node.Left = Left;
            node.Top = Top;

            HostCanvas.Children.Remove(Border);
            HostCanvas.NodeCollection.Add(node);
        }

        private void SelectionNode_MouseLeave(object sender, MouseEventArgs e)
        {
            HostCanvas.Children.Remove(Border);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null) return;

            var selectedType = listBox.SelectedItem as Type;
            if (selectedType == null) return;

            var node = (Node) Activator.CreateInstance(selectedType, HostCanvas);

            node.Left = Left;
            node.Top = Top;

            HostCanvas.Children.Remove(Border);
            HostCanvas.NodeCollection.Add(node);
        }

        public override void Calculate()
        {
        }

        public override Node Clone()
        {
            return (Node) MemberwiseClone();
        }
    }
}