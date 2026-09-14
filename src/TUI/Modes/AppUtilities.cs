using static TUI.Display;

namespace TUI.Modes
{
    internal class AppUtilities
    {
        public static void Start()
        {
            while (true)
            {
                AppOption? op = InputHandler.ChoiceMenu<AppOption>(
                    "Please choose an action to perform"
                );
                switch (op)
                {
                    case AppOption.Clean_Crash_Dumps:
                        bool res = Core.Utils.CrashDump.CleanCrashDump();
                        if (res)
                            WriteMsg("Crash dump cleaned successfully", MsgType.Success);
                        else
                            WriteMsg("Failed to clean the crash dump", MsgType.Error);

                        break;
                    default:
                        return;
                }
            }
        }

        enum AppOption
        {
            Clean_Crash_Dumps,
        }
    }
}
