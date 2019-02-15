using EasyConsole;
using System.Collections.Generic;
using System.Drawing;

using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages
{
    using Extensions;
    using FeatureExpansion;
    using Lib.Controller;
    using Packages;

    public class PackageCreator : UpdatableMenuPage
    {
        private PackageCreator()
            : base("", null, null)
        {
        }

        public PackageCreator(UpdatableProgram program)
            : base("Main Page", program, GetOptions)
        {
        }

        public static IEnumerable<Option> GetOptions(UpdatableProgram program)
        {
            int i = 0;

            foreach (var pkg in PackageController.PackageList)
            {
                yield return new Option(pkg.Name, () => { PackageController.SelectedPackage = i; program.NavigateTo<PackageOperator>(); });
                ++i;
            }

            yield return new Option("New Package", () => program.NavigateTo<PackageAdd>());
            yield return new Option("Remove Package", () => program.GetPage<PackageRemove>().WithSubmenu(PackageController.PackageList).NavigateTo<PackageRemove>());

            if (string.IsNullOrEmpty(SetupController.TesterPath))
                yield return new Option("Locate Tester Path", () => LocateTesterPath(program));
        }

        private static void LocateTesterPath(UpdatableProgram program)
        {
            Console.Write("Write the path to your ZWSetup cloned repository: ");
            string rootFolder = Console.ReadLine();

            SetupController.LocateTester(rootFolder);
            Console.WriteLine("Sucesfully located Tester project.", Color.DarkGreen);
            Console.Read();

            program.RedrawCurrentPage();
        }
    }
}