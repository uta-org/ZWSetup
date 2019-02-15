using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;
    using Lib.Controller;

    public class MainPage : MenuPage
    {
        private MainPage()
            : base("", null)
        {
        }

        public MainPage(UpdatableProgram program)
        : base("Main Page", program, GetOptions(program).ToArray())
        {
        }

        public static IEnumerable<Option> GetOptions(UpdatableProgram program)
        {
            yield return new Option("Enter Package Manager", () => program.NavigateTo<PackageManager>());
            yield return new Option("Enter Package Creator", () => program.NavigateTo<PackageCreator>());
            yield return new Option("Exit Application", SafeExit);
        }

        public static void SafeExit()
        {
            SettingsController.SaveSettings();
            Environment.Exit(0);
        }
    }
}