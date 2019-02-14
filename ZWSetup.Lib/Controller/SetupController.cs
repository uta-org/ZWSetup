using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using uzLib.Lite.Extensions;

namespace ZWSetup.Lib.Controller
{
    public static class SetupController
    {
        public static string TesterPath { get; set; }

        /// <summary>
        /// Checks the if the Tester project path is determined.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     This should be on the Shell.Controller namespace, because I don't expect that to be on Unity. I only pretend to implement package installation through Unity.
        ///     But the Settings are on the Lib so we will use this impl here.
        /// </remarks>
        public static bool CheckTesterPathDetermined()
        {
            // First, check if it can be determined.

            string executingAssemblyFolderName = Path.GetFileName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).ToLower();
            if (executingAssemblyFolderName == "release" || executingAssemblyFolderName == "debug")
            {
                string testerPath = Path.Combine(IOHelper.GoUpInTree(executingAssemblyFolderName, 3), "ZWSetup.Tester", "ZWSetup.Tester.csproj");

                if (!File.Exists(testerPath))
                    throw new Exception("If you're editing this project, please, don't alter Tester path");

                TesterPath = testerPath;

                return true;
            }

            return !string.IsNullOrEmpty(TesterPath);
        }
    }
}