using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace TUM.CMS.VplControl.Watch3D
{
    /// <summary>
    /// 
    /// </summary>
    public class MeshIdandGeometry
    {
        public Guid Id { get; set; }
        public GeometryModel3D Model3D { get; set; }
        public Material Material { get; set; }
    }
}
