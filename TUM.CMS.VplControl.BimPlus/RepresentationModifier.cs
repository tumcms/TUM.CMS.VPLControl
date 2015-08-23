using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BimPlus.Explorer.Contract.Model;
using BimPlus.Explorer.Contract.Services;
using BimPlus.Explorer.Contract.ViewModel.Representation;
using Nemetschek.NUtilLibrary;

namespace TUM.CMS.VplControl.BimPlus
{
    /// <summary>
    ///     /// The modfier takes care to adjust the 3D representations
    /// </summary>
    [Export(ContractNames.RepresentationModifier, typeof (RepresentationModifier))]
    public class RepresentationModifier : IPartImportsSatisfiedNotification, IRepresentationModifier
    {
        /// <summary>
        ///     Model source
        /// </summary>
        [Import(typeof (IDataContainer))]
        private IDataContainer DataContainer { get; set; }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 3D Representation source
        /// </summary>
        public IRepresentationSource RepresentationSource { get; set; }

        void IRepresentationModifier.OnRepresentationContentChanged()
        {
            //throw new NotImplementedException();
        }

        internal void SetObjectsHihglighted(List<Guid> highlightedElements)
        {
            var elements = DataContainer.GetElements();
            if (elements != null && RepresentationSource != null)
            {
                foreach (var elem in elements)
                {
                    var baseElem = elem as BaseElement;
                    if (baseElem == null) continue;
                    var id = baseElem.Id;

                    var repElem = RepresentationSource.GetRepresentation(id);
                    if (repElem == null)
                        continue;

                    repElem.Transparency = (byte) (highlightedElements.Contains(id) ? 100 : 15);
                }
                // notify update
                RepresentationSource.NotifyRepresentationChanged();
            }
            else
            {
                if (elements == null) return;
                foreach (
                    var repElem in
                        elements.OfType<BaseElement>()
                            .Select(baseElem => baseElem.Id)
                            .Where(id => RepresentationSource != null)
                            .Select(id => RepresentationSource.GetRepresentation(id))
                            .Where(repElem => repElem != null))
                {
                    // as default -> set transparent
                    repElem.Transparency = 15;
                }
            }
        }

        // Visualize the Objects with the wanted transparancy of all the other objects
        internal void SetObjectsHihglighted(List<Guid> highlightedElements, int transparency, bool highlighted)
        {
            // Get Color 
            var colorDialog = new ColorDialog();
            if (!highlighted) return;
            colorDialog.ShowDialog();

            var elements = DataContainer.GetElements();
            if (elements != null && RepresentationSource != null)
            {
                foreach (var elem in elements)
                {
                    var baseElem = elem as BaseElement;
                    if (baseElem == null) continue;
                    var id = baseElem.Id;

                    var repElem = RepresentationSource.GetRepresentation(id);
                    if (repElem == null) continue;
                    if (highlightedElements.Contains(id))
                    {
                        // 100 means opaque
                        repElem.Transparency = 100;

                        // Set object Highlighted
                        repElem.Color = colorDialog.Color;
                        repElem.UseColor = true;
                    }
                    else
                    {
                        // as default -> set transparent
                        var trans = BitConverter.GetBytes(transparency);
                        repElem.Transparency = trans[0];
                    }
                }
                // notify update
                RepresentationSource.NotifyRepresentationChanged();
            }
            else
            {
                try
                {
                    if (elements == null) return;
                    foreach (var elem in elements)
                    {
                        var baseElem = elem as BaseElement;
                        if (baseElem == null) continue;
                        var id = baseElem.Id;

                        if (RepresentationSource == null) continue;
                        var repElem = RepresentationSource.GetRepresentation(id);
                        if (repElem == null)
                            continue;
                        // as default -> set transparent
                        var trans = BitConverter.GetBytes(transparency);
                        repElem.Transparency = trans[0];
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        internal void VisualizeNonVisualElements(List<Guid> elementsList)
        {
            var elements = DataContainer.GetElements();
            IList<CElementPolyeder> subMeshes = new List<CElementPolyeder>();

            if (elements == null || RepresentationSource == null) return;
            foreach (var elem in elements)
            {
                var element = elem as BaseElement;
                if (element == null) continue;
                var id = element.Id;

                if (!elementsList.Contains(id)) continue;
                if (element.Polyeder == null) continue;
                var elementPolyeder = new CElementPolyeder();

                elementPolyeder.SetBasePolyeder(element.Polyeder);
                elementPolyeder.ID = element.Id;
                if (element.Material != Color.Transparent)
                    elementPolyeder.color = element.Material;
                else if (element.Polyeder.IsSpecialColorDefined())
                    elementPolyeder.color = Color.FromArgb((int) element.Polyeder.GetSpecialColor());
                else
                    elementPolyeder.color = Color.DimGray;

                subMeshes.Add(elementPolyeder);

                // Create mesh for use in dx control
                // new CDxObjectMesh(element.Id, subMeshes);

                // Pin mesh should not be selectable
                // mesh.type = PolyederType.ColorImmutable;

                //_elementsViewModel

                // Representations.Add(vm);
            }
        }

        public void VisualizeElements(List<Guid> elementsList)
        {
            IList<CElementPolyeder> subMeshes = new List<CElementPolyeder>();

        }

    }
}