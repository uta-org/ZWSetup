﻿using EasyConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;

    public class PackageInstaller : UpdatableMenuPage
    {
        private PackageInstaller()
            : base("", null, null)
        {
        }

        public PackageInstaller(Program program)
            : base("Main Page", program, GetOptions)
        {
        }

        public static IEnumerable<Option> GetOptions(Program program)
        {
            yield return new Option("New package", () => program.NavigateTo<>());
            yield return new Option("Remove package", () => program.NavigateTo<>());
        }
    }
}