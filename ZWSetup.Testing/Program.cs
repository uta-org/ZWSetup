using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWSetup.Shell.Controller;

namespace ZWSetup.Testing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TesterController.GenerateSetupInTester("");
            Console.Read();
        }
    }
}