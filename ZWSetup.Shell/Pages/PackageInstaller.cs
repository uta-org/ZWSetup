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

    public class PackageInstaller : UpdatableMenuPage
    {
        private PackageInstaller()
            : base("", null, null)
        {
        }

        public PackageInstaller(UpdatableProgram program)
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

            yield return new Option("New package", () => program.NavigateTo<PackageAdd>());
            yield return new Option("Remove package", () => program.GetPage<PackageRemove>().WithSubmenu(PackageController.PackageList).NavigateTo<PackageRemove>());
            yield return new Option("Locate Tester path", () => LocateTesterPath(program));
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