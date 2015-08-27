using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BimPlus.Explorer.Contract.Model;
using BimPlus.Sdk.Data.Geometry;
using BimPlus.Sdk.Data.TenantDto;
using Nemetschek.Allready.Logistics.DbCore;
using Nemetschek.NUtilLibrary;
using TUM.CMS.VplControl.BimPlus.Controls;
using TUM.CMS.VplControl.Nodes;
using Brushes = System.Windows.Media.Brushes;

namespace TUM.CMS.VplControl.BimPlus.Nodes
{
    public class ElementContainerNode : Node
    {
        private readonly DataController _controller;
        private List<BaseElement> _elements;

        private BackgroundWorker _worker;

        private ElementContainerNodeControl _control;

        public ElementContainerNode(Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            // Call the Singleton Class to get the actual loaded elements -> Connection to the DataModel
            _controller = DataController.Instance;

            _control = new ElementContainerNodeControl();
            _control.cancelBut.Click += CancelButOnClick;

            AddControlToNode(_control);

            AddInputPortToNode("Input", typeof (object));
            AddOutputPortToNode("Elements", typeof (object));

            DataContext = this;
        }

        private void CancelButOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_worker.IsBusy)
                _worker.CancelAsync();

            Dispatcher.BeginInvoke((Action)delegate ()
            {
                _control.LoadingGrid.Visibility = Visibility.Collapsed;
                _control.statusLabel.Content = "Status: Loading cancelled ...";
            });
        }

        public override void Calculate()
        {
            if (InputPorts[0].Data == null) return;

            _worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            _worker.DoWork += WorkerOnDoWork;
            _worker.ProgressChanged += WorkerOnProgressChanged;
            _worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            _worker.RunWorkerAsync(10000);
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            _control.progressBar.Value = progressChangedEventArgs.ProgressPercentage;

            Dispatcher.BeginInvoke((Action)delegate()
            {
                _control.statusLabel.Content = "Status: Loading ..." + progressChangedEventArgs.ProgressPercentage + " %";
            });

        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            // Shoe ProgressBar
            Dispatcher.BeginInvoke((Action)delegate () {
                _control.LoadingGrid.Visibility = Visibility.Visible;
                _control.statusLabel.Content = "Status: Loading ...";
            });

            if (InputPorts[0] == null || ControlElements[0] == null || InputPorts[0].Data == null) return;

            if (InputPorts[0].Data.GetType() == typeof(Project))
            {
                var project = InputPorts[0].Data as Project;
                if (project != null)
                    _elements = _controller.IntBase.APICore.GetElementsFromTopologyId(project.Id);
                // OutputPorts[0].Data = _elements;
            }

            if (InputPorts[0].Data.GetType() == typeof(DtoDivision))
            {
                var dtoDivision = InputPorts[0].Data as DtoDivision;
                if (dtoDivision?.TopologyDivisionId != null)
                    _elements = _controller.IntBase.APICore.GetElementsFromTopologyId((Guid)dtoDivision.TopologyDivisionId);
            }

            doWorkEventArgs.Result = _elements;
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (_elements != null)
            {
                Dispatcher.BeginInvoke((Action)delegate () {
                    _control.LoadingGrid.Visibility = Visibility.Collapsed;
                    _control.statusLabel.Content = "Status: " +_elements.Count+ " Elements loaded!";
                });
            }

            //  MessageBox.Show("Container Node \n Loading finished!  \n" + _elements.Count + " Elements loaded");

            if (_elements != null) OutputPorts[0].Data = _elements.Cast<GenericElement>().ToList();
        }

        public override Node Clone()
        {
            return new ElementContainerNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }

        public void LoadGeometricRepresentation(List<BaseElement> elements)
        {
            try
            {
                foreach (var baseElem in elements)
                {
                    if(baseElem.Id != Guid.Empty)
                        _controller.IntBase.APICore.GetObjectGeometryAsThreeJs(baseElem.Id);

                    if (baseElem.Id == Guid.Empty)
                        baseElem.Id = Guid.NewGuid();

                    var data = baseElem.AttributeGroups["geometry"].Attributes["picture"].ToString();

                    if (data != null)
                    {
                        var bytePolyeder = Convert.FromBase64String(data);
                        var NULLcompress = Convert.ToInt32(baseElem.AttributeGroups["geometry"].Attributes["compress"]);
                        var compress = (byte) (NULLcompress == null ? 1 : NULLcompress);

                        CBaseElementPolyeder poly = null;
                        try
                        {
                            if (Convert.ToInt32(baseElem.AttributeGroups["geometry"].Attributes["type"]) == 7)
                                poly = CsgGeometry.Create(bytePolyeder, compress);
                            else
                            {
                                poly = DeserializeVarbinaryMax.DeserializePolyeder(bytePolyeder, compress);

                                if (poly.face.Count == 0 && poly.edge.Count > 0)
                                    poly = Tubes.CreateTube(poly);
                            }
                        }
                        catch (Exception)
                        {
                        }

                        var stringMatrix = baseElem.AttributeGroups["geometry"].Attributes["matrix"].ToString();
                        if (!string.IsNullOrEmpty(stringMatrix) && poly != null)
                        {
                            var mat = new CMatrix(Convert.FromBase64String(stringMatrix));
                            poly.MultiplyWithMatrix(mat.Values);
                        }

                        var col = baseElem.AttributeGroups["geometry"].Attributes["color"];
                        try
                        {
                            baseElem.Material = col != null
                                ? Color.FromArgb((int) Convert.ToUInt32(col))
                                : Color.Transparent;
                        }
                        catch (Exception)
                        {
                            baseElem.Material = col != null ? Color.FromArgb(Convert.ToInt32(col)) : Color.Transparent;
                        }

                        if (col == null || baseElem.Material == Color.FromArgb(255, 128, 128, 128))
                        {
                            if (poly != null)
                            {
                                baseElem.Material = poly.IsSpecialColorDefined()
                                    ? Color.FromArgb((int) poly.GetSpecialColor())
                                    : Color.FromArgb(255, 128, 128, 128);
                            }
                            else
                                baseElem.Material = Color.FromArgb(255, 128, 128, 128);
                        }

                        if (poly != null)
                        {
                            poly.ID = baseElem.Id;
                            baseElem.Polyeder = poly;
                            poly.Radius = null;
                            poly.Bendingrolle = null;
                        }
                        else
                        {
                            Debug.Print(
                                $"Invalid geometry for {((GenericElement) baseElem).TypeName} with ID {baseElem.Id}!");
                        }
                    }

                    baseElem.LoadState |= ElementLoadState.GeometryLoaded;
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}