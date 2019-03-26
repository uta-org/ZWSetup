using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;

namespace ZWSetup.Lib.Model
{
    using Controller;

    public sealed class ZTWPackage
    {
        [JsonIgnore]
        public static string Extension { get; } = "ztwp";

        [JsonIgnore]
        public string Name => Path.GetFileNameWithoutExtension(SolutionPath);

        [JsonIgnore]
        public string SetupPath => this.GetSetupFile();

        [JsonIgnore]
        public string SetupFileName => this.GetSetupFileName();

        [JsonIgnore]
        public string PrettyName => Name.ToLowerInvariant().FirstCharToUpper();

        [JsonIgnore]
        public string SetupClass => $"{PrettyName}Setup";

        [JsonIgnore]
        public string SetupNamespace => $"ZWSetup.Package.{PrettyName}";

        [JsonIgnore]
        public string SetupFullname => $"{SetupNamespace}.{SetupClass}";

        [JsonIgnore]
        public bool DoesSetupExists => File.Exists(SetupPath);

        [JsonIgnore]
        public string TempPrefix => $"{PrettyName}_";

        [JsonIgnore]
        public string RelExecutablePath => Path.Combine("Package", GetMainExecutable());

        [JsonProperty]
        public string SolutionPath { get; set; }

        [JsonProperty]
        public List<string> References { get; set; }

        private ZTWPackage()
        {
        }

        public ZTWPackage(string slnPath)
        {
            SolutionPath = slnPath;
        }

        private string GetMainExecutable()
        {
            try
            {
                string projectName = VSHelper.GetStartUpProjectName(SolutionPath);
                return $"{projectName}.exe";
            }
            catch
            {
                Console.WriteLine("Solution must have a startup project in order to get the main executable.", Color.Red);
                throw;
            }
        }
    }
}