using System;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace TUM.CMS.VPL.Scripting.Python
{
    public class PythonScriptCompiler
    {
        private readonly ScriptEngine pyEngine;
        private readonly ScriptScope pyScope;

        public PythonScriptCompiler()
        {
            if (pyEngine != null) return;

            pyEngine = IronPython.Hosting.Python.CreateEngine();
            pyScope = pyEngine.CreateScope();

            var runtime = pyEngine.Runtime;
            runtime.LoadAssembly(typeof (string).Assembly);
            runtime.LoadAssembly(typeof (Uri).Assembly);
            // runtime.LoadAssembly(typeof(Source).Assembly);

            // pyScope.SetVariable("log", _logger);
            // _logger.AddInfo("Python Initialized");
        }

        public object CompileSourceAndExecute(string code, object input)
        {
            if (input != null)
                pyScope.SetVariable("Input", input);
            if (code == null) throw new ArgumentNullException("code");
            var source = pyEngine.CreateScriptSourceFromString
                (code, SourceCodeKind.Statements);
            var compiled = source.Compile();
            // Executes in the scope of Python
            compiled.Execute(pyScope);

            return pyScope.GetVariable("Output") ?? null;
        }
    }
}