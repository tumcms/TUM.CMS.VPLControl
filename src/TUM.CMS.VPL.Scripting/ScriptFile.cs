using System.Collections.ObjectModel;

namespace TUM.CMS.VPL.Scripting
{
    public class ScriptFile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScriptFile" /> class.
        /// </summary>
        public ScriptFile()
        {
            ReferencedAssemblies = new ObservableCollection<string>();
            ScriptContent = string.Empty;
        }

        /// <summary>
        ///     Gets the referenced assemblies.
        /// </summary>
        /// <value>
        ///     The referenced assemblies.
        /// </value>
        public ObservableCollection<string> ReferencedAssemblies { get; private set; }

        /// <summary>
        ///     Gets or sets the content of the script.
        /// </summary>
        public string ScriptContent { get; set; }
    }
}