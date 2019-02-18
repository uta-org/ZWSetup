using Newtonsoft.Json;
using System.IO;
using uzLib.Lite.Extensions;

namespace ZWSetup.Lib.Model
{
    using Controller;

    public sealed class ZTWPackage
    {
        public const string Extension = "ztwp";

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

        [JsonProperty]
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