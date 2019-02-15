using Newtonsoft.Json;

namespace ZWSetup.Lib.Controller
{
    using Properties;

    public static class SettingsController
    {
        private static Settings MySettings => Settings.Default;

        //static SettingsController()
        //{
        //    LoadSettings();
        //}

        public static void LoadSettings()
        {
            string json = MySettings.PackageList;

            if (!string.IsNullOrEmpty(json) && json != "null") // If in SaveSettings we check if PackageList is null or empty, we shouldn't check for "null" literal (but we must don't do this check)
                PackageController.PackageList = JsonConvert.DeserializeObject<PackageList>(json);

            SetupController.TesterPath = MySettings.TesterPath;
        }

        public static void SaveSettings()
        {
            // if (!PackageList.IsNullOrEmpty()) // Don't do this check because if user removes all the listed packages, we will need to save it.
            MySettings.PackageList = JsonConvert.SerializeObject(PackageController.PackageList);
            MySettings.TesterPath = SetupController.TesterPath;

            MySettings.Save();
        }
    }
}