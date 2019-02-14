using EasyConsole;
using System.Collections.Generic;
using System.Linq;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;

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
            yield return new Option("Enter package manager", () => program.NavigateTo<PackageManager>());
            yield return new Option("Install package", () => program.NavigateTo<PackageInstaller>());
        }
    }
}