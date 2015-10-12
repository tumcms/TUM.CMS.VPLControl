using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using BimPlus.IntegrationFramework.Contract.Model;
using TUM.CMS.VplControl.BimPlus.Controls;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class RelationNode : Node
    {
        private ObservableCollection<Tuple<object, object>> _relationElements;

        public RelationNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            DataContext = this;

            AddInputPortToNode("Input", typeof(object));
            AddOutputPortToNode("Relation", typeof (object));

            _relationElements = new ObservableCollection<Tuple<object, object>>
            {
                new Tuple<object, object>(1, "TEST"),
                new Tuple<object, object>(2, "TEST"),
                new Tuple<object, object>(3, "TEST"),
                new Tuple<object, object>(4, "TEST")
            };

            var nodeControl = new RelationNodeControl
            {
                Margin = new Thickness(5, 20, 5, 15),
                ListBox = {ItemsSource = _relationElements, Width = 150}
            };

            AddControlToNode(nodeControl);

            BinButton.Visibility = Visibility.Visible;
        }

        public override void Calculate()
        {
            // Input Part
            if (InputPorts[0].Data.GetType() != typeof (Project)) return;
            // _modelComboBox.ItemsSource = null;

            var project = InputPorts[0].Data as Project;
            if (project == null) return;

            // Output Part
            // if (_modelComboBox != null) OutputPorts[0].Data = _modelComboBox.SelectedItem as Division;
        }

        public override Node Clone()
        {
            return new RelationNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }

        #region PropertyChangedHandlers
        public ObservableCollection<Tuple<object, object>> RelationElements
        {
            get { return _relationElements; }
            set
            {
                _relationElements = value;
                OnPropertyChanged("RelationElements");
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        private new void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion PropertyChangedHandlers
    }
}