using EasyConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using uzLib.Lite.Extensions;

//using System.Drawing;

using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;
    using Extensions;
    using Lib.Model;

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

            // Download the repo (if the user agrees)
            string packagePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(downloadString));

            if(!File.Exists(packagePath))
                NetHelper.DownloadFile(downloadString, packagePath);

            // Extract the package under Local > UTA > ZWSetup folder (where the user.config is) > crete a folder for this package and extract it
            string destination = GetDestination(name);

            if(destination.IsDirectoryEmptyOrNull())
                CompressionHelper.Unzip(packagePath, destination);

            // Execute OnSetup
            var pkg = ParsePackage(destination);
            Assembly asm;
            if (!PackageHelper.GetAssembly(pkg.SetupPath, out asm))
            {
                CurrentProgram.Exit();
                return;
            }

            asm.InvokeStaticMethod(pkg.SetupFullname, "OnSetup");

            // Exit after executing OnSetup
            CurrentProgram.Exit();
        }

        private static string GetDestination(string packageName)
        {
            string destination = Path.Combine(ProgramHelper.ConfigPath, packageName);

            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            return destination;
        }

        // TODO: Remove this (create a mapping JSON file for dependencies, *.cs used files && ZTWPackage instance, like a SLN file)
        private static ZTWPackage ParsePackage(string destination)
        {
            var infoJSON = Directory.GetFiles(destination, "info.json", SearchOption.TopDirectoryOnly);

            if (infoJSON.Length == 0)
                throw new Exception("Unable to find info.json -- package is corrupt!");

            return JsonConvert.DeserializeObject<ZTWPackage>(File.ReadAllText(infoJSON[0]));
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