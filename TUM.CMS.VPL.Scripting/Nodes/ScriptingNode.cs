using System;
using System.CodeDom.Compiler;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using TUM.CMS.VplControl.Nodes;
using TUM.CMS.VPL.Scripting.CSharp;
using TUM.CMS.VPL.Scripting.Python;

namespace TUM.CMS.VPL.Scripting.Nodes
{
    public class ScriptingNode : Node
    {
        private readonly Scripting.ScriptingControl scriptingControl;
        private string scriptContent;

        public ScriptingNode(VplControl.Core.VplControl hostCanvas)
            : base(hostCanvas)
        {
            scriptingControl = new Scripting.ScriptingControl();


            scriptingControl.HighlightingComboBox.SelectionChanged += HighlightingComboBoxOnSelectionChanged;


            // Create new script File
            scriptingControl.CurrentFile = new CSharpScriptFile();
            scriptingControl.Height = 400;
            scriptingControl.Width = 700;
            scriptingControl.DockPanel.Height = 400;


            scriptingControl.StartCompilingEventHandler += StartCompilingEventHandler;

            scriptingControl.StartCSharpCompilingEventHandler += StartCSharpCompilation;
            scriptingControl.StartPythonCompilingEventHandler += StartPythonCompilation;

            AddControlToNode(scriptingControl);

            AddInputPortToNode("Input", typeof (object));

            AddOutputPortToNode("Output", typeof (object));
        }

        private void HighlightingComboBoxOnSelectionChanged(object sender,
            SelectionChangedEventArgs selectionChangedEventArgs)
        {
            switch (scriptingControl.HighlightingComboBox.SelectedItem.ToString())
            {
                case "C#":
                    scriptingControl.CurrentFile = new CSharpScriptFile();
                    break;
                case "Python":
                    scriptingControl.CurrentFile = new PythonScriptFile();
                    break;
            }
            scriptingControl.TextEditor.Text = scriptingControl.CurrentFile.ScriptContent;
        }

        private void StartCompilingEventHandler(object sender, EventArgs eventArgs)
        {
            Calculate();
        }

        public void StartCSharpCompilation(object sender, EventArgs eventArgs)
        {
            if (TopComment.Visibility == Visibility.Visible)
            {
                TopComment.Visibility = Visibility.Hidden;
                TopComment.HostNode_PropertyChanged(null, null);
            }

            scriptingControl.CurrentFile.ScriptContent = scriptingControl.TextEditor.Text;

            // Define the Input for the Script
            if (InputPorts[0].Data != null)
            {
                // Define the Input for the Script
            }

            try
            {
                var mScriptCompiler = new Lazy<CSharpScriptCompiler>();
                //Compile and execute current script
                var result = mScriptCompiler.Value.Compile(scriptingControl.CurrentFile as CSharpScriptFile);

                if (result.GetType() == typeof (CompilerErrorCollection))
                {
                    var compilerErrors = new StringBuilder();
                    var compilerErrorCollection = result as CompilerErrorCollection;
                    if (compilerErrorCollection == null)
                    {
                        throw new InvalidOperationException(
                            "Unable to compile scripts: " +
                            compilerErrors);
                    }
                    foreach (var actError in compilerErrorCollection)
                        compilerErrors.Append("" + actError + Environment.NewLine);
                    TopComment.Text = compilerErrors.ToString();
                    TopComment.Visibility = Visibility.Visible;
                    TopComment.HostNode_PropertyChanged(null, null);
                    throw new InvalidOperationException(
                        "Unable to compile scripts: " +
                        compilerErrors);
                }

                OutputPorts[0].Data = result;
            }
            catch (Exception e)
            {
            }
        }

        public void StartPythonCompilation(object sender, EventArgs eventArgs)
        {
            if (TopComment.Visibility == Visibility.Visible)
            {
                TopComment.Visibility = Visibility.Hidden;
                TopComment.HostNode_PropertyChanged(null, null);
            }

            var file = new PythonScriptFile {ScriptContent = scriptingControl.TextEditor.Text};

            try
            {
                var compiler = new PythonScriptCompiler();

                var obj = compiler.CompileSourceAndExecute(file.ScriptContent, InputPorts[0].Data);
                if (obj != null)
                    OutputPorts[0].Data = obj;
            }
            catch (Exception e)
            {
                TopComment.Text = e.ToString();
                TopComment.Visibility = Visibility.Visible;
                TopComment.HostNode_PropertyChanged(null, null);
            }
        }

        public override void Calculate()
        {
        }

        public override void SerializeNetwork(XmlWriter xmlWriter)
        {
            base.SerializeNetwork(xmlWriter);

            scriptContent = scriptingControl.TextEditor.Text;

            xmlWriter.WriteStartAttribute("_mSkriptContent");
            xmlWriter.WriteValue(scriptContent);
            xmlWriter.WriteEndAttribute();
        }

        public override void DeserializeNetwork(XmlReader xmlReader)
        {
            base.DeserializeNetwork(xmlReader);
            scriptingControl.TextEditor.Text = xmlReader.GetAttribute("_mSkriptContent");
        }

        public override Node Clone()
        {
            return new ScriptingNode(HostCanvas)
            {
                Top = Top,
                Left = Left
            };
        }
    }
}