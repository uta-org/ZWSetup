﻿namespace ZWSetup.Shell
{
    using FeatureExpansion;
    using Pages;
    using Pages.Packages;

    public class MainProgram : UpdatableProgram
    {
        public MainProgram()
            : base("ZWSetup", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));
            AddPage(new PackageInstaller(this));
            AddPage(new PackageManager(this));
            AddPage(new PackageAdd(this));
            AddPage(new PackageRemove(this));

            SetPage<MainPage>();
        }
    }
}