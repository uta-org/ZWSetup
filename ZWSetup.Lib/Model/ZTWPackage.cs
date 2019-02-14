using System.IO;

namespace ZWSetup.Lib.Model
{
    public sealed class ZTWPackage
    {
        public string Name => Path.GetFileNameWithoutExtension(SolutionPath);

        public string SolutionPath { get; set; }

        private ZTWPackage()
        {
        }

        public ZTWPackage(string slnPath)
        {
            SolutionPath = slnPath;
        }
    }
}