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

            Console.WriteLine(outputLog, isSuccesful ? Color.DarkGreen : Color.Red);

            if (!isSuccesful)
            {
                Exit();
                return;
            }

            //string errorStr;
            //if (!CompileSolution(pkg.SolutionPath, outputDir, out errorStr))
            //{
            //    Console.WriteLine(errorStr, Color.Red);

            //    Exit();
            //    return;
            //}

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

        //private static bool CompileSolution(string solutionPath, string outputDir)
        //{
        //    return CompileSolution(solutionPath, outputDir, false);
        //}

        //private static bool CompileSolution(string solutionPath, string outputDir, bool outputErrors = true)
        //{
        //    string errorStr;
        //    return CompileSolution(solutionPath, outputDir, out errorStr, false);
        //}

        //private static bool CompileSolution(string solutionPath, string outputDir, out string errorStr)
        //{
        //    return CompileSolution(solutionPath, outputDir, out errorStr, false);
        //}

        //private static bool CompileSolution(string solutionPath, string outputDir, out string errorStr, bool outputErrors = true)
        //{
        //    errorStr = "";
        //    bool success = true;

        //    //MSBuildLocator.RegisterInstance(RoslynHelper.GetMSBuildInstance());

        //    // Thanks to: https://stackoverflow.com/a/29550838/3286975
        //    var props = new Dictionary<string, string>();
        //    props["CheckForSystemRuntimeDependency"] = "true";

        //    EmitResult result = null;
        //    MSBuildWorkspace workspace = MSBuildWorkspace.Create(props);
        //    Solution solution = workspace.OpenSolutionAsync(solutionPath).Result;
        //    ProjectDependencyGraph projectGraph = solution.GetProjectDependencyGraph();

        //    // Thanks to: https://stackoverflow.com/q/46032472/3286975
        //    workspace.WorkspaceFailed += (sender, eventArgs) =>
        //    {
        //        Console.WriteLine($"{eventArgs.Diagnostic.Kind}: {eventArgs.Diagnostic.Message}", Color.Red);
        //        Console.WriteLine();
        //    };

        //    // workspace.LoadMetadataForReferencedProjects = true;

        //    RoslynHelper.StartDebugging();

        //    foreach (ProjectId projectId in projectGraph.GetTopologicallySortedProjects())
        //    {
        //        var project = solution.GetProject(projectId);
        //        CSharpCompilation projectCompilation = project.GetCompilationAsync().Result as CSharpCompilation; //).DoRequired(project);

        //        projectCompilation.FindAllMissingReferences(solution, project);

        //        string projectPath = project.FilePath;
        //        Project evProject = !string.IsNullOrEmpty(projectPath) ? new Project(projectPath) : null;

        //        bool isDLL = evProject == null || evProject.GetItems("OutputType").Any(item => item.ToString() == "Library");

        //        if (projectCompilation != null && !string.IsNullOrEmpty(projectCompilation.AssemblyName))
        //        {
        //            using (var stream = new MemoryStream())
        //            using (var pdbStream = new MemoryStream())
        //            {
        //                result = projectCompilation.Emit(stream, pdbStream);

        //                if (result.Success)
        //                {
        //                    // Test (exe or dll)
        //                    string fileName = $"{projectCompilation.AssemblyName}.{(isDLL ? "dll" : "exe")}",
        //                           pdbFile = $"{projectCompilation.AssemblyName}.pdb";

        //                    stream.WriteToFile(outputDir, fileName);
        //                    pdbStream.WriteToFile(outputDir, pdbFile);
        //                }
        //                else
        //                    success = false;
        //            }
        //        }
        //        else
        //            success = false;

        //        if (!success)
        //        {
        //            errorStr = result?.VerifyCompilationResults();
        //            if (outputErrors)
        //                Console.WriteLine(errorStr, Color.Red);
        //        }
        //    }

        //    RoslynHelper.EndDebugging();

        //    return success;
        //}

        private static void Exit()
        {
            Console.WriteLine("Press any key to go back...");
            Console.Read();

            CurrentProgram.NavigateBack();
        }
    }
}