namespace ZWSetup.Lib.Controller
{
    using Lib.Model;

    public static class PackageController
    {
        public static ZTWPackage CurrentPackage => SelectedPackage != -1 ? PackageList[SelectedPackage] : null;
        public static int SelectedPackage { get; set; }
        public static PackageList PackageList { get; private set; }

        static PackageController()
        {
            PackageList = new PackageList();
            SelectedPackage = -1;
        }
    }
}