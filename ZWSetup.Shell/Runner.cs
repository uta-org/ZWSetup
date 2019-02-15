using System.Runtime.InteropServices;

namespace ZWSetup.Shell
{
    using Lib.Controller;

    internal class Runner
    {
        // TODO: Make a extension for this

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private static EventHandler _handler;

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    SettingsController.SaveSettings();
                    return false;
            }
        }

        private static void Main(string[] args)
        {
            SettingsController.LoadSettings();

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            new MainProgram().Run();
        }
    }
}