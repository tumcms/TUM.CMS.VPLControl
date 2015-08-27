using System.IO;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.IO;

namespace TUM.CMS.VplControl.IFC.Utilities
{
    public class IfcReader
    {
        public IfcReader()
        {
            
        }

        public void ReadIfc(string filepath)
        {
            var ifcFileName = filepath;

            var tempFilename = Path.GetTempFileName();
            var model = new XbimModel();

            model.CreateFrom(ifcFileName, tempFilename);
            model.Open(tempFilename);

            var products = model.Instances.OfType<IfcProduct>();
            var storeys = model.Instances.OfType<IfcBuildingStorey>().ToList();
        }
    }
}
