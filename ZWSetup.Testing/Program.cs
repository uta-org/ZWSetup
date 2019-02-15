using System;
using ZWSetup.Shell.Controller;

namespace ZWSetup.Testing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            TesterController.GenerateSetupInTester("test");
            Console.Read();
        }
    }
}