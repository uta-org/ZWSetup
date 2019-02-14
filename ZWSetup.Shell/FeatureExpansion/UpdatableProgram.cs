using EasyConsole;

using System;
using System.Linq;

namespace ZWSetup.Shell.FeatureExpansion
{
    public class UpdatableProgram : Program
    {
        protected UpdatableProgram(string title, bool breadcrumbHeader)
            : base(title, breadcrumbHeader)
        {
        }

        protected Page CurrentPage => (History.Any()) ? History.Peek() : null;

        public new T NavigateTo<T>() where T : Page
        {
            (CurrentPage as UpdatableMenuPage).UpdateOptions();

            SetPage<T>();

            Console.Clear();
            CurrentPage.Display();
            return CurrentPage as T;
        }

        public new Page NavigateBack()
        {
            (CurrentPage as UpdatableMenuPage).UpdateOptions();

            History.Pop();

            Console.Clear();
            CurrentPage.Display();
            return CurrentPage;
        }
    }
}