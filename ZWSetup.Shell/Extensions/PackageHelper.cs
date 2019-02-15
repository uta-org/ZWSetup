using EasyConsole;
using System;
using System.Collections.Generic;

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
    }
}