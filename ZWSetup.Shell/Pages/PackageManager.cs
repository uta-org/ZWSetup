using EasyConsole;
using System.Drawing;

using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages
{
    public class PackageManager : MenuPage
    {
        private PackageManager()
            : base("", null)
        {
        }

        public PackageManager(Program program)
            : base("Main Page", program)
        {
        }

        public override void Display()
        {
            Console.WriteLine("This page is still in development. Going back after pressing any key...", Color.Yellow);
            Console.Read();

            Program.NavigateBack();
        }
    }
}