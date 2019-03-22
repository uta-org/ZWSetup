using EasyConsole;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using uzLib.Lite.Extensions;

using Console = Colorful.Console;

//using Header = System.Tuple<string, string>;

namespace ZWSetup.Shell.Pages
{
    using FeatureExpansion;
    using Extensions;

    public class PackageManager : UpdatableMenuPage
    {
        private const string SearchURL = "https://api.github.com/search/repositories?q=topic:zwtp";
        private const string AcceptHeader = "application/vnd.github.cloak-preview";
        //private static Header AcceptHeader { get; } = new Header("Accept", "application/vnd.github.cloak-preview");

        // public string APIResponse { get; private set; }
        public static JObject JSONObject { get; private set; }

        private PackageManager()
            : base("", null, null)
        {
        }

        public PackageManager(UpdatableProgram program)
            : base("Main Page", program, GetOptions)
        {
        }

        public static IEnumerable<Option> GetOptions(UpdatableProgram program)
        {
            JSONObject = PackageHelper.GetObjectFromAPIResponse(SearchURL.MakeRequest(AcceptHeader));

            if (JSONObject == null)
                yield break;

            foreach (var repoItem in JSONObject["items"].Cast<JObject>())
                yield return new Option(repoItem["name"].ToObject<string>(), null);
        }

        /*public override void Display()
        {
            //Console.WriteLine("This page is still in development. Going back after pressing any key...", Color.Yellow);
            //Console.Read();

            // This doesn't be casted as UpdatableProgram
            Program.NavigateBack();
        }*/
    }
}