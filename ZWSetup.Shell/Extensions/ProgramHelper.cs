using EasyConsole;
using System;

namespace ZWSetup.Shell.Extensions
{
    using FeatureExpansion;

    public static class ProgramHelper
    {
        private const string OnlyUpdatableProgramException = "This extension must be used only with UpdatableProgram.";

        public static void NavigateTo<T>(this Page page)
            where T : Page
        {
            if (!(page.Program is UpdatableProgram))
                throw new Exception(OnlyUpdatableProgramException);

            (page.Program as UpdatableProgram).NavigateTo<T>();
        }

        public static void NavigateBack(this Page page)
        {
            if (!(page.Program is UpdatableProgram))
                throw new Exception(OnlyUpdatableProgramException);

            (page.Program as UpdatableProgram).NavigateBack();
        }
    }
}