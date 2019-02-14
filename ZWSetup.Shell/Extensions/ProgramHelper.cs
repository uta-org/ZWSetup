using EasyConsole;

namespace ZWSetup.Shell.Extensions
{
    using FeatureExpansion;

    public static class ProgramHelper
    {
        public static void NavigateTo<T>(this Page page)
            where T : Page
        {
            page.Program.NavigateTo<T>();
        }

        public static void NavigateBack(this Page page)
        {
            page.Program.NavigateBack();
        }
    }
}