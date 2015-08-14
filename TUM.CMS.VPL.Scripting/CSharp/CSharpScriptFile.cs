using System;

namespace TUM.CMS.VPL.Scripting.CSharp
{
    public class CSharpScriptFile : ScriptFile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CSharpScriptFile" /> class.
        /// </summary>
        public CSharpScriptFile()
        {
            ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            ReferencedAssemblies.Add("System.dll");
            ReferencedAssemblies.Add("System.Core.dll");
            ReferencedAssemblies.Add("System.Data.dll");
            ReferencedAssemblies.Add("System.Linq.dll");
            ReferencedAssemblies.Add("System.Windows.Forms.dll");

            ScriptContent =
                "using System;" + Environment.NewLine +
                "using System.Collections.Generic; " + Environment.NewLine +
                "using System.Linq; " + Environment.NewLine +
                "using System.Text; " + Environment.NewLine +
                "using System.Threading.Tasks; " + Environment.NewLine +
                "using System.Windows.Forms; " + Environment.NewLine +
                "" + Environment.NewLine +
                "public class ScriptedClass" + Environment.NewLine +
                "{" + Environment.NewLine +
                "    public static void Execute()" + Environment.NewLine +
                "    {" + Environment.NewLine +
                "        MessageBox.Show(\"Hello World! from script\");" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "}" + Environment.NewLine;
        }
    }
}