using EasyConsole;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages.Packages
{
    using FeatureExpansion;
    using Lib.Controller;
    using Lib.Model;

    public class PackageOperator : MenuPage
    {
        private const string TesterException = "Couldn't find ZTWPackage Setup path.";

        private static Program CurrentProgram { get; set; }

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
            CurrentProgram = program;
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

            if (!CheckTester(pkg))
                throw new Exception(TesterException);

            CodeDomProvider objCodeCompiler = new CSharpCodeProvider();
            CompilerParameters objCompilerParameters = new CompilerParameters();
            CompilerResults results = objCodeCompiler.CompileAssemblyFromFile(objCompilerParameters, setupPath);

            if (!results.Errors.Cast<CompilerError>().IsNullOrEmpty())
            {
                foreach (CompilerError error in results.Errors)
                    Console.WriteLine(error);

                ExitTest();
                return;
            }

            var asm = results.CompiledAssembly;

            asm.InvokeStaticMethod(pkg.SetupFullname, "OnSetup");

            ExitTest();
        }

        private static void ExitTest()
        {
            Console.WriteLine("Press any key to go back...");
            Console.Read();

            CurrentProgram.NavigateBack();
        }

        private static void Compile()
        {
            // Generate the ztwp file

            var pkg = PackageController.CurrentPackage;

            if (!CheckTester(pkg))
                throw new Exception(TesterException);
        }

        private static bool CheckTester(ZTWPackage pkg)
        {
            // This will check if the SetupPath of any pkg exists in HDD
            return File.Exists(pkg.SetupPath);
        }
    }
}