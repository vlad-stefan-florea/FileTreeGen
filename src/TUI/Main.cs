using System.Reflection;
using Core.Utils;
using static TUI.Display;

namespace TUI
{
    public class Main
    {
        private static string Title = """
                _______ __   ______               ______         
               / ____(_) /__/_  __/_______  ___  / ____/__  ____ 
              / /_  / / / _ \/ / / ___/ _ \/ _ \/ / __/ _ \/ __ \
             / __/ / / /  __/ / / /  /  __/  __/ /_/ /  __/ / / /
            /_/   /_/_/\___/_/ /_/   \___/\___/\____/\___/_/ /_/ 
            """;
        private static string AppHeaderInfo =
            "Version: " + AppInfo.Version + " | Developed by: " + AppInfo.Developer;

        public static async Task Run()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            WriteColor(Title, ConsoleColor.Green);
            WriteColor(AppHeaderInfo, ConsoleColor.Gray);

            while (true)
            {
                ModeOption? mode = InputHandler.ChoiceMenu<ModeOption>("What do you wish to do?");
                switch (mode)
                {
                    case ModeOption.Generate_Folder_Reports:
                        await Modes.Scan.Start();
                        break;
                    case ModeOption.App_Utilities:
                        Modes.AppUtilities.Start();
                        break;
                    default:
                        return;
                }
            }
        }
    }

    enum ModeOption
    {
        Generate_Folder_Reports,
        App_Utilities,
    }
}
