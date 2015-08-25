using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BimPlus.Explorer.Contract.Model;
using TUM.CMS.VplControl.BimPlus.Controls;
using TUM.CMS.VplControl.Nodes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class IssueNode : Node
    {
        private readonly DataController _controller;
        private readonly IssueNodeControl _issueControl;
        // private List<BaseElement> _elements;

        public IssueNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            _issueControl = new IssueNodeControl();
            _issueControl.BaseButtonClicked += IssueControlOnBaseButtonClicked;

            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            AddInputPortToNode("Project/Model", typeof (object));
            AddInputPortToNode("LinkedElements", typeof (object));
            AddInputPortToNode("GeometryView", typeof (object));

            // Is this correct? There can be different members in different projects ... 
            _issueControl.ResponsibleUserComboBox.ItemsSource = _controller.DataContainer.GetTeamMembers();
            _issueControl.ResponsibleUserComboBox.DisplayMemberPath = "User";

            AddControlToNode(_issueControl);
            DataContext = this;
        }

        public override void Calculate()
        {
        }

        private void IssueControlOnBaseButtonClicked(object sender, EventArgs eventArgs)
        {
            var issue = new Issue();

            if (InputPorts[0].Data == null || InputPorts[1].Data == null || InputPorts[2].Data == null) return;
            // Common Information
            issue.CreatedAt = DateTime.Now;
            issue.Author = _controller.DataContainer.GetCurrentUserName();
            issue.Id = Guid.NewGuid();

            // User Control Stuff
            issue.Description = _issueControl.DescriptionTextBox.Text;
            var teamMember = _issueControl.ResponsibleUserComboBox.SelectedItem as TeamMember;
            if (teamMember != null)
                issue.Responsible = teamMember.User;

            var project = InputPorts[0].Data as Project;
            
            // Create the Issue
            if (project != null)
            {
                issue.ProjectId = project.Id;
                _controller.IntBase.CreateIssue(issue, issue.Id, project.Id);
            }
               
            Issue createdIssue = null;

            // Download the Issue
            if (project == null) return;

            foreach (var item in _controller.IntBase.GetIssues(project.Id).Where(item => item.Id == issue.Id))
            {
                createdIssue = item;
            }

            // Screenshot Stuff
            var image = RenderFrameworkElement(InputPorts[2].ConnectedConnectors[0].StartPort.ParentNode);

            var encoder = new JpegBitmapEncoder();
            var photolocation = _controller.IntBase.UserDirectory + Guid.NewGuid() + ".jpg"; //file name 

            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var filestream = new FileStream(photolocation, FileMode.Create))
                encoder.Save(filestream);

            var fs = File.OpenRead(photolocation);
            var bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

            try
            {
                if (createdIssue != null)
                    createdIssue.AddAttachment(photolocation, bytes);
            }
            catch (Exception)
            {
                // ignored
            }

            // Pin Stuff
            if (InputPorts[1].Data.GetType() != typeof (List<GenericElement>)) return;
            var genericElements = InputPorts[1].Data as List<GenericElement>;
            if (genericElements == null) return;
            foreach (var item in genericElements)
            {
                var pin = new Pin
                {
                    ObjectId = item.Id
                };
                try
                {
                    issue.AddPin(pin);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public override Node Clone()
        {
            return new IssueNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }

        /// <summary>
        /// Function for the rendering of a various frameworkelement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static BitmapImage RenderFrameworkElement(FrameworkElement element)
        {
            var width = element.ActualWidth;
            var height = element.ActualHeight;
            var bmpCopied = new RenderTargetBitmap((int) Math.Round(width), (int) Math.Round(height), 96, 96,
                PixelFormats.Default);
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                var vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);

            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bmpCopied));
            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}