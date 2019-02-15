using EasyConsole;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace ZWSetup.Shell.Pages.Packages
{
    using FeatureExpansion;
    using Lib.Controller;

    public class PackageOperator : MenuPage
    {
        // Menu Page
        //
        // Options:
        // Test
        // Compile (check if the Setup.cs has the needed methods + compile)
        // For any of this option check the Tester. Because, we need it, to solve Setup.cs file

        private PackageOperator()
            : base("", null)
        {
        }

        public PackageOperator(UpdatableProgram program)
            : base("Package Operator", program, GetOptions(program).ToArray())
        {
        }

        public static IEnumerable<Option> GetOptions(UpdatableProgram program)
        {
            yield return new Option("Test", Test);
            yield return new Option("Compile", Compile);
        }

        private static void Test()
        {
            // Retrieve the selected package
            var pkg = PackageController.CurrentPackage;

            string setupPath = pkg.SetupPath;

            CodeDomProvider objCodeCompiler = new CSharpCodeProvider();
            CompilerParameters objCompilerParameters = new CompilerParameters();
            CompilerResults results = objCodeCompiler.CompileAssemblyFromFile(objCompilerParameters, setupPath);

            foreach (var outStr in results.Output)
                Console.WriteLine(outStr);
        }

        private static void Compile()
        {
            // Generate the ztwp file
        }

        private static bool CheckTester()
        {
            return false;
        }
    }
}