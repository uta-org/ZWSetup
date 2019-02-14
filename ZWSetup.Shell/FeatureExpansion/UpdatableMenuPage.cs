using EasyConsole;

using System;
using System.Linq;

namespace ZWSetup.Shell.FeatureExpansion
{
    /// <summary>
    ///  This will be used to make MenuPages updatable
    /// </summary>
    public class UpdatableMenuPage : MenuPage
    {
        protected new UpdatableMenu Menu { get; set; }

        private UpdatableMenuPage()
            : base("", null)
        {
        }

        protected UpdatableMenuPage(string title, Program program)
            : base(title, program)
        {
        }

        public T SetOptions<T>(params Option[] options)
            where T : UpdatableMenuPage
        {
            Menu.Options = options.ToList();

            return (T)this;
        }

        public override void Display()
        {
            if (Program.History.Count > 1 && Program.BreadcrumbHeader)
            {
                string breadcrumb = null;
                foreach (var title in Program.History.Select((page) => page.Title).Reverse())
                    breadcrumb += title + " > ";
                breadcrumb = breadcrumb.Remove(breadcrumb.Length - 3);
                Console.WriteLine(breadcrumb);
            }
            else
                Console.WriteLine(Title);

            Console.WriteLine("---");

            if (Program.NavigationEnabled && !Menu.Contains("Go back"))
                Menu.Add("Go back", () => { Program.NavigateBack(); });

            Menu.Display();
        }
    }
}