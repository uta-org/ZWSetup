using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWSetup.Shell.Pages.Packages.Interfaces
{
    using Lib.Model;

    public interface IPackage<T>
    {
        ZTWPackage CurrentPackage { get; set; }

        T SetPackage(ZTWPackage package);
    }
}