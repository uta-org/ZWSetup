using EasyConsole;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ZWSetup.Shell.FeatureExpansion
{
    public class UpdatableProgram : Program
    {
        private Dictionary<Type, Page> Pages { get; set; }

        protected UpdatableProgram(string title, bool breadcrumbHeader)
            : base(title, breadcrumbHeader)
        {
            Pages = new Dictionary<Type, Page>();
        }

        protected Page CurrentPage => (History.Any()) ? History.Peek() : null;

        public new void AddPage(Page page)
        {
            Type pageType = page.GetType();

            if (Pages.ContainsKey(pageType))
                Pages[pageType] = page;
            else
                Pages.Add(pageType, page);
        }

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

        public T GetPage<T>()
            where T : Page
        {
            Type pageType = typeof(T);

            Page nextPage;
            if (!Pages.TryGetValue(pageType, out nextPage))
                throw new KeyNotFoundException($"The given page \"{pageType.Name}\" was not present in the program");

            return (T)nextPage;
        }
    }
}