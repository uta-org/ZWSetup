using EasyConsole;
using System.Drawing;

using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages.Packages
{
    using FeatureExpansion;
    using Interfaces;
    using Lib.Controller;
    using Lib.Model;

    public class PackageRemove : MenuPage, IPackage<PackageRemove>
    {
        public ZTWPackage CurrentPackage { get; set; }

        private PackageRemove()
            : base("", null)
        {
        }

        public PackageRemove(UpdatableProgram program)
            : base("Remove Package", program)
        {
        }

        public override void Display()
        {
            string name = PackageController.RemoveCurrent();
            Console.Write($"Removed package '{name}' succesfully! Press any key to go back...", Color.DarkGreen);
            Console.Read();

            (Program as UpdatableProgram).NavigateBack();
        }

        public PackageRemove SetPackage(ZTWPackage package)
        {
            CurrentPackage = package;
            return this;
        }
    }
}