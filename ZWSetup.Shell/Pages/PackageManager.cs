using EasyConsole;
using System.Drawing;

using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;

    public class PackageManager : MenuPage
    {
        private PackageManager()
            : base("", null)
        {
        }

        public PackageManager(UpdatableProgram program)
            : base("Main Page", program)
        {
        }

        public override void Display()
        {
            Console.WriteLine("This page is still in development. Going back after pressing any key...", Color.Yellow);
            Console.Read();

            // This doesn't be casted as UpdatableProgram
            Program.NavigateBack();
        }
    }
}