using Newtonsoft.Json;
using uzLib.Lite.Extensions;

namespace ZWSetup.Lib.Controller
{
    using Lib.Model;
    using Properties;

    public static class PackageController
    {
        // Implement settings here after we solved the update problem

        private static Settings MySettings => Settings.Default;
        public static ZTWPackage CurrentPackage => SelectedPackage != -1 ? PackageList[SelectedPackage] : null;
        public static int SelectedPackage { get; set; }
        public static PackageList PackageList { get; private set; }

        static PackageController()
        {
            PackageList = new PackageList();
            SelectedPackage = -1;

            LoadSettings();
        }

        private static void LoadSettings()
        {
            string json = MySettings.PackageList;

            if (!string.IsNullOrEmpty(json) && json != "null") // If in SaveSettings we check if PackageList is null or empty, we shouldn't check for "null" literal (but we must don't do this check)
                PackageList = JsonConvert.DeserializeObject<PackageList>(json);
        }

        public static void SaveSettings()
        {
            // if (!PackageList.IsNullOrEmpty()) // Don't do this check because if user removes all the listed packages, we will need to save it.
            MySettings.PackageList = JsonConvert.SerializeObject(PackageList);

            MySettings.Save();
        }

        /// <summary>
        /// Adds a new package following the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static ZTWPackage Add(string path)
        {
            var package = new ZTWPackage(path);
            PackageList.Add(package);

            return package;
        }

        /// <summary>
        /// Removes the current package.
        /// </summary>
        /// <returns>The package name.</returns>
        public static string RemoveCurrent()
        {
            string name = CurrentPackage.Name;
            PackageList.Remove(CurrentPackage);
            SelectedPackage = -1;

            return name;
        }

        /// <summary>
        /// Removes the specified package by its index.
        /// </summary>
        /// <param name="index">The index.</param>
        public static void Remove(int index)
        {
            PackageList.RemoveAt(index);
        }
    }
}