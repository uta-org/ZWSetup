using EasyConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using uzLib.Lite.Extensions;

//using System.Drawing;

using Console = Colorful.Console;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;
    using Extensions;

    public class PackageManager : UpdatableMenuPage
    {
        private const string SearchURL = "https://api.github.com/search/repositories?q=topic:zwtp";
        private const string AcceptHeader = "application/vnd.github.cloak-preview";

        public static JObject JSONObject { get; private set; }

        private PackageManager()
            : base("", null, null)
        {
        }

        public PackageManager(UpdatableProgram program)
            : base("Main Page", program, GetOptions)
        {
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

            // TODO: Extract the package under Local > UTA > ZWSetup folder (where the user.config is) > crete a folder for this package and extract it

            // TODO: Execute OnSetup (it will display hello world!)

            Console.WriteLine(downloadString);
            Console.Read();
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