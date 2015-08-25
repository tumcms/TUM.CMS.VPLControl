﻿// Component Model for MEF Import / Export

using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using BimPlus.Explorer.Contract.Services;
using BimPlus.Explorer.Contract.ViewModel.Gui;
using BimPlus.IntegrationFramework;
using BimPlus.IntegrationFramework.ViewModels;

// Bimplus Services -> DataProvider

namespace TUM.CMS.VplControl.BimPlus
{
    [Export("TUM.CMS.VplControl.BimPlus.DataController")]
    public sealed class DataController : IPartImportsSatisfiedNotification
    {
        // Singleton
        private static DataController _instance;
        private static readonly object Padlock = new object();

        private DataController()
        {
        }

        [Import(typeof (IDataContainer))]
        public IDataContainer DataContainer { get; set; }

        [ImportMany(global::BimPlus.Explorer.Contract.ContractNames.App.TabElements, typeof(ITabElement), AllowRecomposition = true)]
        public ObservableCollection<ITabElement> TabElements = new ObservableCollection<ITabElement>();

        [Import(typeof (IntegrationBase))]
        public IntegrationBase IntBase { get; set; }

        [Import(ContractNames.RepresentationModifier)]
        public RepresentationModifier RepMod { get; set; }

        [Import(global::BimPlus.IntegrationFramework.ContractNames.StatusViewModel)]
        public StatusViewModel StatusViewModel { get; set; }

        public ContentPresenter ProjContPres { get; set; }

        public static DataController Instance
        {
            get
            {
                lock (Padlock)
                {
                    return _instance ?? (_instance = new DataController());
                }
            }
        }

        public void OnImportsSatisfied()
        {
            Instance.DataContainer = DataContainer;
            // Instance.ProjContPres = new ContentPresenter {Content = ProjectSelectionViewModel};
            Instance.IntBase = IntBase;

            /*
            foreach (var item in _applicationButtons.Where(item => item.GetType() == typeof(BimPlus.Explorer.GeometryView.StartButton)))
            {
                // geomBut = item as AbstractApplicationButton;
            }
            Instance.geomContPres = new ContentPresenter { Content = geomBut.ViewModel };
            */
        }
    }
}