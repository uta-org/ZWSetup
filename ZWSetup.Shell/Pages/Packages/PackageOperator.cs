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
            yield return new Option("Add new references", AddReferences);
            yield return new Option("Create ztwp file (compile)", Compile);
        }

        private static void AddReferences()
        {
            // Retrieve the selected package
            var pkg = PackageController.CurrentPackage;
            bool contains = false;
            string reference = "";
            int count = 0;

            do
            {
                if (contains || string.IsNullOrWhiteSpace(reference) && count > 0)
                    Console.Clear();

                Console.Write("Write path or name of your DLL file: ");
                reference = Console.ReadLine();

                contains = !pkg.References.IsNullOrEmpty() && pkg.References.Any(r => r == reference);

                if (contains)
                {
                    Console.WriteLine($"Reference '{reference}' is already present on this package ({pkg.PrettyName})!", Color.Red);
                    AnyKeyRetry();
                }

                if (string.IsNullOrWhiteSpace(reference))
                {
                    Console.WriteLine("Reference can't be null!", Color.Red);
                    AnyKeyRetry();
                }
            }
            while (contains || string.IsNullOrWhiteSpace(reference));

            // TODO: Check if valid reference (or if you need to resolve it)

            pkg.References = pkg.References.AddSafe(reference);

            Console.WriteLine($"Succesfully added '{reference}' as reference!", Color.DarkGreen);
            CurrentProgram.Exit();
        }

        private static void AnyKeyRetry()
        {
            Console.WriteLine();
            Console.Write("Press any key to retry...");
            Console.Read();
        }

        private static void Test()
        {
            // Retrieve the selected package
            var pkg = PackageController.CurrentPackage;

            if (!pkg.DoesSetupExists)
                throw new Exception(TesterException);

            Assembly asm;
            if (!PackageHelper.GetAssembly(pkg, out asm))
            {
                CurrentProgram.Exit();
                return;
            }

            // TODO: There isn't any destination to get full path as parameter
            asm.InvokeStaticMethod(pkg.SetupFullname, PackageConsts.OnSetupMethod, pkg.RelExecutablePath);

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
                if (!PackageHelper.GetAssembly(pkg, out asm))
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

                // We will only copy the *.cs (hint: OnSetup) file.
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