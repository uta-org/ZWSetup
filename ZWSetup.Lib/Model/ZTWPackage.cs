using Newtonsoft.Json;
using System.IO;
using uzLib.Lite.Extensions;

namespace ZWSetup.Lib.Model
{
    using Controller;

    public sealed class ZTWPackage
    {
        [JsonIgnore]
        public string Name => Path.GetFileNameWithoutExtension(SolutionPath);

        [JsonIgnore]
        public string SetupPath => this.GetSetupFile();

        [JsonIgnore]
        public string PrettyName => Name.ToLower().FirstCharToUpper();

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