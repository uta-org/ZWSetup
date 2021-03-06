﻿namespace ZWSetup.Shell.Pages.Packages.Interfaces
{
    using Lib.Model;

    public interface IPackage<T>
    {
        ZTWPackage CurrentPackage { get; set; }

        T SetPackage(ZTWPackage package);
    }
}