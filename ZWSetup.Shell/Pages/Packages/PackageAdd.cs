using EasyConsole;
using System;
using System.IO;
using System.Drawing;

using Console = Colorful.Console;

using uzLib.Lite.Extensions;

namespace ZWSetup.Shell.Pages.Packages
{
    using Interfaces;
    using Lib.Model;
    using Lib.Controller;

    public class PackageAdd : MenuPage, IPackage<PackageAdd>
    {
        public ZTWPackage CurrentPackage { get; set; }

        private PackageAdd()
            : base("", null)
        {
        }

        public PackageAdd(Program program)
            : base("Add New Package", program)
        {
        }

        public override void Display()
        {
            Console.Write("SLN File Path: ");
            string path = Console.ReadLine();

            if (path.IsDirectory())
            {
                var files = Directory.GetFiles(path, "*.sln");

                if (files.Length == 0)
                {
                    Console.WriteLine("The provided folder doesn't have any folder!", Color.Red);
                    Program.NavigateBack();
                    return;
                }

                path = files[0];
            }

            if (!File.Exists(path))
            {
                Console.WriteLine("The file you provided doesn't exists!", Color.Red);
                Program.NavigateBack();
                return;
            }

            ZTWPackage package = PackageController.Add(path);
            Console.WriteLine($"Added package '{package.Name}' succesfully!", Color.DarkGreen);

            Program.NavigateBack();
        }

        public PackageAdd SetPackage(ZTWPackage package)
        {
            CurrentPackage = package;
            return this;
        }
    }
}