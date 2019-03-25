using EasyConsole;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using uzLib.Lite.Extensions;
using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages.Packages
{
    using Controller;
    using FeatureExpansion;
    using Lib.Controller;
    using Lib.Model;
    using Extensions;

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
            if (!PackageHelper.GetAssembly(pkg.SetupPath, out asm))
            {
                CurrentProgram.Exit();
                return;
            }

            asm.InvokeStaticMethod(pkg.SetupFullname, "OnSetup");

            CurrentProgram.Exit();
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
                if (!PackageHelper.GetAssembly(pkg.SetupPath, out asm))
                {
                    CurrentProgram.Exit();
                    return;
                }

                if (!asm.HasMethod(pkg.SetupFullname, PackageConsts.OnSetupMethod) || !asm.HasMethod(pkg.SetupFullname, PackageConsts.OnFinishMethod))
                {
                    Console.WriteLine($"There are missing methods on '{pkg.SetupFullname}' in '{pkg.SetupPath}'. Please, add them before trying to compile.", Color.Red);

                    CurrentProgram.Exit();
                    return;
                }
            }

            // Step 1: Create a temp folder where everything will be stored
            string tempFolder = IOHelper.GetTemporaryDirectory(pkg.TempPrefix, "", false),
                   outputDir = CreateFolderStructure(tempFolder);

            // Step 2: Compile solution to generate a exe file (csc)
            bool isSuccesful = CompilerHelper.Compile(pkg.SolutionPath, outputDir);

            if (!isSuccesful)
            {
                CurrentProgram.Exit();
                return;
            }

            // Note: The structure of the package will be as following:
            // root
            // info.json
            // (pkgName)Setup.cs
            // Package/
            // files (exe, libs, etc etc)

            // Step 3: Search for the Setup of pkg and copy on this folder and generate needed files
            {
                if (!pkg.DoesSetupExists)
                    throw new Exception(TesterException);

                // We will only copy the *Setup.cs file.
                File.Copy(pkg.SetupPath, Path.Combine(tempFolder, pkg.SetupFileName));

                // Then, we will generate info.json and copy into the folder
                File.WriteAllText(Path.Combine(tempFolder, "info.json"), JsonConvert.SerializeObject(pkg));
            }

            // Step 4: Zip everything into a ".ztwp" extension file
            string compressedFile = CompressionHelper.Zip(tempFolder, Path.GetTempPath(), ZTWPackage.Extension);

            // Step 4.1: Remove Directory
            if (!IOHelper.EmptyFolder(tempFolder))
                throw new Exception("Couldn't empty the temp folder!");

            // Step 5: Open file in Explorer

            //Process.Start($"file://{compressedFile}"); // Search or work in a cross-platform solution for this
            Process.Start(Path.GetTempPath());

            CurrentProgram.Exit();
        }

        private static string CreateFolderStructure(string tempFolder)
        {
            string path = Path.Combine(tempFolder, "Package");
            Directory.CreateDirectory(path);

            return path;
        }
    }
}