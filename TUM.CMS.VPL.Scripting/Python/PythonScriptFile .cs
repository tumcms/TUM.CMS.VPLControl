using System;
using TUM.CMS.VPL.Scripting.CSharp;

namespace TUM.CMS.VPL.Scripting.Python
{
    public class PythonScriptFile : ScriptFile

    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CSharpScriptFile" /> class.
        /// </summary>
        public PythonScriptFile()
        {
            ScriptContent =
                "import clr" + Environment.NewLine + Environment.NewLine +
                "import System" + Environment.NewLine +
                "clr.AddReference('System.Windows.Forms')" + Environment.NewLine +
                "from System.Windows.Forms import *" + Environment.NewLine + Environment.NewLine +
                "# Welcome to the IronPython Framework" + Environment.NewLine +
                "# For returning a value please use the 'Output' variable" + Environment.NewLine +
                "# The input object via the port is automatically set to the 'Input' variable" + Environment.NewLine +
                Environment.NewLine +
                "MessageBox.Show('Hello World!')" + Environment.NewLine +
                "Output = 1 + Input";
        }
    }
}