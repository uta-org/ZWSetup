﻿using System;
using System.IO;
using System.Reflection;
using uzLib.Lite.Extensions;

namespace ZWSetup.Lib.Controller
{
    public static class SetupController
    {
        public static string TesterPath { get; internal set; }

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

        public static void LocateTester(string rootFolder)
        {
            if (!rootFolder.IsDirectory())
                throw new ArgumentException("The specified path must be a folder (the root folder of a cloned repository of ZWSetup).", "rootFolder");

            var slnFiles = Directory.GetFiles(rootFolder, "ZWSetup.sln");

            if (slnFiles.Length == 0)
                throw new ArgumentException("The specified path must be the root folder of your ZWSetup cloned repository.", "rootFolder");

            TesterPath = Path.Combine(rootFolder, "ZWSetup.Tester", "ZWSetup.Tester.csproj");
        }
    }
}