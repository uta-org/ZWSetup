using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWSetup.Shell.Controller
{
    using Lib;

    public static class PackageController
    {
        public static PackageList PackageList { get; private set; }

        static PackageController()
        {
            PackageList = new PackageList();
        }
    }
}