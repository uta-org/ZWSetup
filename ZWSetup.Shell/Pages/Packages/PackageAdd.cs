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
    using FeatureExpansion;

    public class PackageAdd : MenuPage
    {
        private PackageAdd()
            : base("", null)
        {
        }

        public PackageAdd(UpdatableProgram program)
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
                    (Program as UpdatableProgram).NavigateBack();
                    return;
                }

                path = files[0];
            }

            if (!File.Exists(path))
            {
                Console.WriteLine("The file you provided doesn't exists!", Color.Red);
                (Program as UpdatableProgram).NavigateBack();
                return;
            }

            ZTWPackage package = PackageController.Add(path);

            // Then check if we have located Tester project... To append a new item to its csproj
            if (!SetupController.CheckTesterPathDetermined())
            {
                Console.Write("Couldn't determine the Tester csproj file, please, determine it.", Color.Yellow);
                Console.Read();
                return;
            }

            Console.Write($"Added package '{package.Name}' succesfully! Press any key to go back...", Color.DarkGreen);
            Console.Read();

            (Program as UpdatableProgram).NavigateBack();
        }
    }
}