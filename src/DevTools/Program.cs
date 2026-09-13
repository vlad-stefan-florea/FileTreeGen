using Core.Utils;
using static TUI.Display;
using static TUI.InputHandler;

namespace DevTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string rootPath = SolutionUtils.GetFTGRootPath();
            DrawLine('-', 30);
            WriteColor($"{AppInfo.AppName} v{AppInfo.Version} DEVELOPER TOOLS", ConsoleColor.Green);
            DrawLine('-', 10);
            WriteColor("FileTreeGen root path: ", ConsoleColor.Cyan, newLine: false);
            Console.WriteLine(rootPath);
            DrawLine('-', 10);
            ToolOption? tool;
            while (true)
            {
                tool = ChoiceMenu<ToolOption>("Developer tool executor:");
                switch (tool)
                {
                    case ToolOption.Icon_Map_Generator:
                        Tools.IconMapGen.Run();
                        break;
                    default:
                        return;
                }
            }
        }

        enum ToolOption
        {
            Icon_Map_Generator,
        }
    }
}
