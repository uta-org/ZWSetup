using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;

    public class MainPage : UpdatableMenuPage
    {
        public MainPage(Program program)
        : base("Main Page", program, GetOptions)
        {
        }

        public static IEnumerable<Option> GetOptions(Program program)
        {
            yield return new Option("Enter package manager", () => program.NavigateTo<Page1>());
            yield return new Option("Install package", () => program.NavigateTo<Page1>());
        }
    }
}