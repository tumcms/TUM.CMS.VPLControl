using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using BimPlus.Explorer.Contract.Model;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class ProjectNode : Node
    {
        // DataController
        private readonly DataController _controller;
        private readonly ComboBox _projectComboBox;
        private Project _selectedProject;

        public ProjectNode(VplControl hostCanvas)
            : base(hostCanvas)
        {
            DataContext = this;

            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            if (_controller.DataContainer != null)
            {
                _projectComboBox = new ComboBox
                {
                    ItemsSource = _controller.DataContainer.GetProjects(),
                    Width = 100,
                    SelectedItem = _selectedProject,
                    DisplayMemberPath = "Name",
                    Margin = new Thickness(5, 20, 5, 15)
                };

                // Add EventHandler
                _projectComboBox.SelectionChanged += SelectionChanged;

                if (_controller.DataContainer != null)
                    AddControlToNode(_projectComboBox);
                else
                    AddControlToNode(new TextBox {Text = "No Connection"});
            }

            AddOutputPortToNode("Project", typeof (object));
        }

        public override void Calculate()
        {
            // var project = _projectComboBox.Items.CurrentItem as Project;
            if (_projectComboBox.SelectedItem != null)
                OutputPorts[0].Data = _projectComboBox.SelectedItem;
        }

        public override Node Clone()
        {
            return new ProjectNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }

        public void SelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            /*
            var project = _projectComboBox.SelectedItem as Project;
            if (project == null) return;
                _controller.DataContainer.SetCurrentProject(project);
            */

            Calculate();
        }

        #region PropertyChangedHandlers

        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                OnPropertyChanged("SelectedProject");
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