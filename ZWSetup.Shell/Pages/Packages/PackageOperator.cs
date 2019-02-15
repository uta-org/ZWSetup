using EasyConsole;
using System.Collections.Generic;
using System.Linq;

namespace ZWSetup.Shell.Pages.Packages
{
    using FeatureExpansion;

    public class PackageOperator : MenuPage
    {
        // Menu Page
        //
        // Options:
        // Test
        // Compile (check if the Setup.cs has the needed methods + compile)

        private PackageOperator()
            : base("", null)
        {
        }

        public PackageOperator(UpdatableProgram program)
            : base("Package Operator", program, GetOptions(program).ToArray())
        {
        }

        public static IEnumerable<Option> GetOptions(UpdatableProgram program)
        {
            yield break;
        }
    }
}