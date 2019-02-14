using EasyConsole;
using System.Collections.Generic;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;

    public class MainPage : UpdatableMenuPage
    {
        private MainPage()
            : base("", null, null)
        {
        }

        public MainPage(UpdatableProgram program)
        : base("Main Page", program, GetOptions)
        {
        }

        public static IEnumerable<Option> GetOptions(UpdatableProgram program)
        {
            yield return new Option("Enter package manager", () => program.NavigateTo<PackageManager>());
            yield return new Option("Install package", () => program.NavigateTo<PackageInstaller>());
        }
    }
}