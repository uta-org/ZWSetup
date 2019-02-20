using EasyConsole;
using Microsoft.CSharp;

//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Emit;
//using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using uzLib.Lite.Extensions;
using Console = Colorful.Console;

//using Project = Microsoft.Build.Evaluation.Project;

namespace ZWSetup.Shell.Pages.Packages
{
    using Controller;
    using FeatureExpansion;
    using Lib.Controller;
    using Lib.Model;

    //using Microsoft.CodeAnalysis.CSharp;

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
            yield return new Option("Create ztwp file (compile)", Compile);
        }

        private static void Test()
        {
            // Retrieve the selected package
            var pkg = PackageController.CurrentPackage;

            if (!pkg.DoesSetupExists)
                throw new Exception(TesterException);

            Assembly asm;
            if (!GetAssembly(pkg.SetupPath, out asm))
                return;

            asm.InvokeStaticMethod(pkg.SetupFullname, "OnSetup");

            Exit();
        }

        private static bool GetAssembly(string path, out Assembly assembly, bool outputErrors = true)
        {
            bool hasErrors;
            CompilerResults results = GetCompilerResults(path, outputErrors, out hasErrors);

            if (hasErrors)
            {
                assembly = null;
                return false;
            }

            assembly = results.CompiledAssembly;
            return true;
        }

        private static CompilerResults GetCompilerResults(string path, bool outputErrors)
        {
            bool hasErrors;
            return GetCompilerResults(path, outputErrors, out hasErrors);
        }

        private static CompilerResults GetCompilerResults(string path, bool outputErrors, out bool hasErrors)
        {
            CodeDomProvider objCodeCompiler = new CSharpCodeProvider();
            CompilerParameters objCompilerParameters = new CompilerParameters();
            CompilerResults results = objCodeCompiler.CompileAssemblyFromFile(objCompilerParameters, path);

            if (!results.Errors.Cast<CompilerError>().IsNullOrEmpty())
            {
                if (outputErrors)
                    foreach (CompilerError error in results.Errors)
                        Console.WriteLine(error);

                Exit();

                hasErrors = true;
                return null;
            }

            hasErrors = false;
            return results;
        }

        private static void Compile()
        {
            // Generate the ztwp file

            var pkg = PackageController.CurrentPackage;

            if (!pkg.DoesSetupExists)
                throw new Exception(TesterException);

            // Step 0: Check if setup file has the needed methods (OnSetup && OnFinish)
            {
                Assembly asm;
                if (!GetAssembly(pkg.SetupPath, out asm))
                    return;

                if (!asm.HasMethod(pkg.SetupFullname, PackageConsts.OnSetupMethod) || !asm.HasMethod(pkg.SetupFullname, PackageConsts.OnFinishMethod))
                {
                    Console.WriteLine($"There are missing methods on '{pkg.SetupFullname}' in '{pkg.SetupPath}'. Please, add them before trying to compile.", Color.Red);

                    Exit();
                    return;
                }
            }

            // Step 1: Create a temp folder where everything will be stored
            string tempFolder = IOHelper.GetTemporaryDirectory(pkg.TempPrefix),
                   outputDir = CreateFolderStructure(tempFolder);

            // Step 2: Compile solution to generate a exe file (csc)
            string outputLog;
            bool isSuccesful = CompilerHelper.Compile(pkg.SolutionPath, outputDir, out outputLog);

            Console.WriteLine(!isSuccesful ? "Has errors." : "Is succesful.", !isSuccesful ? Color.Red : Color.DarkGreen);
            Console.WriteLine(outputLog, isSuccesful ? Color.DarkGreen : Color.Red);

            if (!isSuccesful)
            {
                Exit();
                return;
            }

            // Note: The structure of the package will be as following:
            // root
            // (pkgName)Setup.cs
            // Package/
            // files (exe, libs, etc etc)

            // Step 3: Search for the Setup of pkg and copy on this folder
            {
                if (!pkg.DoesSetupExists)
                    throw new Exception(TesterException);

                // We will only copy the Setup file.
                File.Copy(pkg.SetupPath, Path.Combine(tempFolder, pkg.SetupFileName));
            }

            // Step 4: Zip everything into a ".ztwp" extension file
            string compressedFile = CompressionHelper.Zip(tempFolder, Path.GetTempPath(), ZTWPackage.Extension);

            // Step 5: Open file in Explorer

            //Process.Start($"file://{compressedFile}"); // Search or work in a cross-platform solution for this
            Process.Start(Path.GetTempPath());

            Exit();
        }

        private static string CreateFolderStructure(string tempFolder)
        {
            string path = Path.Combine(tempFolder, "Package");
            Directory.CreateDirectory(path);

            return path;
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to go back...");
            Console.Read();

            CurrentProgram.NavigateBack();
        }
    }
}