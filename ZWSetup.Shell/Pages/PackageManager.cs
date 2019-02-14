using EasyConsole;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Program.NavigateBack();
        }
    }
}