using EasyConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using uzLib.Lite.Extensions;

//using System.Drawing;

using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;
    using Extensions;
    using System.CodeDom.Compiler;
    using System.Reflection;
    using Microsoft.CSharp;

    public class PackageManager : UpdatableMenuPage
    {
        private const string SearchURL = "https://api.github.com/search/repositories?q=topic:zwtp";
        private const string AcceptHeader = "application/vnd.github.cloak-preview";

        public static JObject JSONObject { get; private set; }

        private static UpdatableProgram CurrentProgram { get; set; }

        private PackageManager()
            : base("", null, null)
        {
        }

        public PackageManager(UpdatableProgram program)
            : base("Package Manager (select any package to download)", program, GetOptions)
        {
            CurrentProgram = program;
        }

        private static IEnumerable<Option> GetOptions(UpdatableProgram program)
        {
            JSONObject = PackageHelper.GetObjectFromAPIResponse(SearchURL.MakeRequest(AcceptHeader));

            if (JSONObject == null)
                yield break;

            foreach (var repoItem in JSONObject["items"].Cast<JObject>())
                yield return new Option(repoItem["name"].ToObject<string>(), () => SelectRepo(repoItem));
        }

        private static void SelectRepo(JObject repoItem)
        {
            string name = repoItem["name"].ToObject<string>(),
                   url = repoItem["owner"]["html_url"].ToObject<string>(),
                   full_url = $"{url}/{name}",
                   downloadString = GetDownloadLink(repoItem["full_name"].ToObject<string>());

            // TODO: Download the repo (if the user agrees)
            string packagePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadString));

            if(!File.Exists(packagePath))
                NetHelper.DownloadFile(downloadString, packagePath);

            // TODO: Extract the package under Local > UTA > ZWSetup folder (where the user.config is) > crete a folder for this package and extract it
            string destination = GetDestination(name);

            if(destination.IsDirectoryEmptyOrNull())
                CompressionHelper.Unzip(packagePath, destination);

            // TODO: Execute OnSetup (it will display hello world!)
            string setupFile = GetSetupFile(destination);
            Assembly asm;
            if (!PackageHelper.GetAssembly(setupFile, out asm))
            {
                CurrentProgram.Exit();
                return;
            }

            // asm.InvokeStaticMethod(pkg.SetupFullname, "OnSetup");

            Console.WriteLine(downloadString);
            Console.Read();
        }

        private static string GetDestination(string packageName)
        {
            string destination = Path.Combine(ProgramHelper.ConfigPath, packageName);

            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            return destination;
        }

        // TODO: Remove this (create a mapping JSON file for dependencies, *.cs used files && ZTWPackage instance, like a SLN file)
        private static string GetSetupFile(string destination)
        {
            var files = Directory.GetFiles(destination, "*.cs", SearchOption.TopDirectoryOnly);

            return files.FirstOrDefault(f => f.Contains("Setup"));
        }

        private static string GetDownloadLink(string fullname)
        {
            string apiURL = $"https://api.github.com/repos/{fullname}/releases";

            var jArr = JsonConvert.DeserializeObject<JArray>(apiURL.MakeRequest());

            // TODO: Ensure the last release download
            // TODO: Ensure that the selected release has the needed "ztwp" extension
            return (jArr[0]["assets"] as JArray)[0]["browser_download_url"].ToObject<string>();
        }
    }
}