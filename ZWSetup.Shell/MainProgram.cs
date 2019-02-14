using EasyConsole;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWSetup.Shell
{
    using FeatureExpansion;
    using Pages;

    public class MainProgram : UpdatableProgram
    {
        public MainProgram()
            : base("ZWSetup", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));

            SetPage<MainPage>();
        }
    }
}