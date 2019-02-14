namespace ZWSetup.Shell
{
    using FeatureExpansion;
    using Pages;

    public class MainProgram : UpdatableProgram
    {
        public MainProgram()
            : base("ZWSetup", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));

            SetPage<MainPage>();
        }
    }
}