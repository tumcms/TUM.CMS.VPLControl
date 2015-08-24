using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TUM.CMS.VPL.Scripting.CSharp
{
    public class CSharpScriptCompiler
    {
        private readonly CodeDomProvider codeCompiler;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CSharpScriptCompiler" /> class.
        /// </summary>
        public CSharpScriptCompiler()
        {
            var compilerParameters = new Dictionary<string, string>();
            compilerParameters["CompilerVersion"] = "v4.0";
            codeCompiler = CodeDomProvider.CreateProvider(
                "cs",
                compilerParameters);
        }

        // ***********
        // Old Version
        // ***********
        /// <summary>
        ///     Compiles given source file.
        /// </summary>
        /// <param name="mCurrentFile">The source file to be compiled.</param>
        /// public Action Compile(CSharpScriptFile mCurrentFile)

        // OutputAssembly = "ScriptedAssembly"
        public object Compile(CSharpScriptFile mCurrentFile)
        {
            //Set compiler parameters
            var compilerParameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                OutputAssembly = AppDomain.CurrentDomain.BaseDirectory + "ScriptedAssembly.dll"
            };
            compilerParameters.ReferencedAssemblies.AddRange(
                mCurrentFile.ReferencedAssemblies.ToArray());

            //Compile the code and return the result or throw an exception in case of an error
            var results = codeCompiler.CompileAssemblyFromSource(
                compilerParameters,
                mCurrentFile.ScriptContent);

            // Get the Compiler Errors
            if (results.Errors.Count > 0)
                return results.Errors;
            if (results.CompiledAssembly == null)
            {
                throw new InvalidOperationException(
                    "Unable to compile scripts: " +
                    "No result assembly from compiler!");
            }

            //Try to get main class
            var compiledAssembly = results.CompiledAssembly;
            var scriptClass = compiledAssembly.GetType("ScriptedClass");
            if (scriptClass == null)
            {
                throw new InvalidOperationException(
                    "Unable to compile scripts: " +
                    "class ScriptedClass not found!");
            }

            //Try to get main method
            var scriptMethod = scriptClass.GetMethod(
                "Execute",
                BindingFlags.Static | BindingFlags.Public);
            if (scriptMethod == null)
            {
                throw new InvalidOperationException(
                    "Unable to compile scripts: " +
                    "method Execute not found within class ScriptedClass!");
            }

            // ***********
            // Old Version
            // ***********
            // return () => scriptMethod.Invoke(null, null);
            var a = scriptMethod.Invoke(scriptMethod.ReturnParameter, null);
            return a;
        }
    }
}