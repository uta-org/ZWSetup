using EasyConsole;
using System.Collections.Generic;

namespace ZWSetup.Shell.Pages
{
    using Extensions;
    using Packages;
    using FeatureExpansion;
    using Lib.Controller;

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
                yield return new Option(pkg.Name, () => PackageController.SelectedPackage = i);
                ++i;
            }

            yield return new Option("New package", () => program.GetPage<PackageAdd>().SetPackage(PackageController.CurrentPackage).NavigateTo<PackageAdd>());
            yield return new Option("Remove package", () => program.GetPage<PackageRemove>().SetPackage(PackageController.CurrentPackage).NavigateTo<PackageRemove>());
        }
    }
}