using EasyConsole;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using uzLib.Lite.Extensions;

namespace ZWSetup.Shell.Extensions
{
    using FeatureExpansion;
    using Lib.Controller;
    using Lib.Model;
    using Pages.Packages.Interfaces;

    public static class PackageHelper
    {
        public static T WithSubmenu<T>(this T packageAware, IEnumerable<ZTWPackage> packages)
            where T : MenuPage, IPackage<T>
        {
            if (!(packageAware.Program is UpdatableProgram))
                throw new Exception("Program from package related page provided must be updatable. Change impl to 'UpdatableProgram'.");

            Console.Clear();

            // First, show package options

            int i = 0;
            var menu = new Menu();
            foreach (var pkg in packages)
            {
                int iCopy = i; // Thanks to: https://stackoverflow.com/a/271447/3286975
                // IntOption should be used here (NOT LONGER NEEDED)
                menu.Add(new Option(pkg.Name, () => PackageController.SelectedPackage = iCopy));
                ++i;
            }

            menu.Display();

            // Then, get page, and setpackage
            return (packageAware.Program as UpdatableProgram).GetPage<T>().SetPackage(PackageController.CurrentPackage);
        }

        public static JObject GetObjectFromAPIResponse(string jsonResponse)
        {
            return JsonConvert.DeserializeObject<JObject>(jsonResponse);
        }

        internal static void Exit(this Program CurrentProgram)
        {
            Console.WriteLine("Press any key to go back...");
            Console.Read();

            CurrentProgram.NavigateBack();
        }

        // ===================================================================================
        // =================              Compile *.cs files                 =================
        // ===================================================================================

        internal static bool GetAssembly(ZTWPackage package, out Assembly assembly, bool outputErrors = true)
        {
            bool hasErrors;
            CompilerResults results = GetCompilerResults(package, outputErrors, out hasErrors);

            if (hasErrors)
            {
                assembly = null;
                return false;
            }

            assembly = results.CompiledAssembly;
            return true;
        }

        internal static CompilerResults GetCompilerResults(ZTWPackage package, bool outputErrors)
        {
            bool hasErrors;
            return GetCompilerResults(package, outputErrors, out hasErrors);
        }

        internal static CompilerResults GetCompilerResults(ZTWPackage package, bool outputErrors, out bool hasErrors)
        {
            var provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            var refs = package.References;

            if (refs != null)
                parameters.ReferencedAssemblies.AddRange(refs.ToArray());

            CompilerResults results = provider.CompileAssemblyFromFile(parameters, package.SetupPath);

            if (!results.Errors.Cast<CompilerError>().IsNullOrEmpty())
            {
                if (outputErrors)
                    foreach (CompilerError error in results.Errors)
                        Console.WriteLine(error);

                hasErrors = true;
                return null;
            }

            hasErrors = false;
            return results;
        }

        // ===================================================================================
    }
}