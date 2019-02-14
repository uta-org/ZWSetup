using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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