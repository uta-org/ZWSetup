using EasyConsole;
using System.Collections.Generic;

namespace ZWSetup.Shell.Pages
{
    using Extensions;
    using Lib.Model;
    using Packages;
    using FeatureExpansion;
    using Controller;

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
            ZTWPackage package = null;

            foreach (var pkg in PackageController.PackageList)
            {
                yield return new Option(pkg.Name, () => package = PackageController.PackageList[i]);
                ++i;
            }

            yield return new Option("New package", () => program.GetPage<PackageAdd>().SetPackage(package).NavigateTo<PackageAdd>());
            yield return new Option("Remove package", () => program.GetPage<PackageRemove>().SetPackage(package).NavigateTo<PackageRemove>());
        }
    }
}