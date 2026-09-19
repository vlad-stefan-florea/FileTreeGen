using Core.Utils;
using static CLI.CommandData;
using static CLI.Types;
using static TUI.Display;

namespace CLI
{
    internal class HelpMenu
    {
        static int defaultPadding = 10;

        public static void DisplayHelp()
        {
            string msg = $"FileTreeGen v{AppInfo.Version} - HELP MENU";
            DrawLine('-', msg.Length + 2);
            Console.WriteLine(msg);
            DrawLine('-', msg.Length + 2);
            DisplayCommand(Root.Cmd, 0);
            Console.WriteLine();
        }

        private static void DisplayCommand(Command cmd, int level)
        {
            Console.Write(new string(' ', level * 3));
            WriteColor(cmd.Name, ConsoleColor.Blue, newLine: false);
            WriteColor(" USAGE", ConsoleColor.Cyan);
            Console.Write(new string(' ', (level + 1) * 3));
            WriteColor(cmd.Syntax, ConsoleColor.Yellow, newLine: false);

            bool hasArgs = cmd.Arguments != null && cmd.Arguments.Count != 0,
                hasOptions = cmd.Options != null && cmd.Options.Count != 0,
                hasSubcmds = cmd.Subcommands != null && cmd.Subcommands.Count != 0;

            // arguments
            string args = string.Empty;
            if (hasArgs)
            {
                args = " ";
                args += string.Join(' ', cmd.Arguments.Select(x => x.Placeholder).ToArray());
                WriteColor(args, ConsoleColor.Gray, newLine: false);
            }
            Console.WriteLine(new string(' ', defaultPadding) + cmd.Description);
            if (hasArgs)
            {
                WriteColor(new string(' ', (level + 1) * 3) + "Arguments:", ConsoleColor.Cyan);
                int maxNameLength = cmd.Arguments.Max(x => x.Placeholder.Length);
                foreach (var arg in cmd.Arguments)
                {
                    Console.Write(new string(' ', (level + 2) * 3));
                    WriteColor(arg.Placeholder, ConsoleColor.Gray, newLine: false);
                    Console.WriteLine(
                        new string(' ', maxNameLength - arg.Placeholder.Length + defaultPadding)
                            + arg.Description
                    );
                }
            }

            // options
            if (hasOptions)
            {
                WriteColor(new string(' ', (level + 1) * 3) + "Options:", ConsoleColor.Cyan);
                DisplayOptions(cmd.Options, level);
            }

            // subcommands
            if (hasSubcmds)
            {
                WriteColor(new string(' ', (level + 1) * 3) + "Subcommands:", ConsoleColor.Cyan);

                foreach (var subcmd in cmd.Subcommands)
                    DisplayCommand(subcmd, level + 2);
            }
        }

        private static void DisplayOptions(List<Option> data, int level)
        {
            int maxNameLength = data.Max(x =>
                x.Syntax.Length + (x.Aliases != null ? x.Aliases.Sum(alias => alias.Length + 2) : 0)
            );
            foreach (var opt in data.OrderBy(x => x.Syntax))
            {
                int aliasesLength = 0;
                var type = GetFlagType(opt.FlagName);
                Console.Write(new string(' ', (level + 2) * 3));
                WriteColor(opt.Syntax, ConsoleColor.Magenta, newLine: false);
                if (opt.Aliases != null)
                {
                    foreach (string a in opt.Aliases)
                    {
                        Console.Write(", ");
                        WriteColor(a, ConsoleColor.DarkMagenta, newLine: false);
                        aliasesLength += a.Length + 2;
                    }
                }
                if (type != null && type.IsEnum)
                {
                    string placeholder = " <value>";
                    WriteColor(placeholder, ConsoleColor.Gray, newLine: false);
                    Console.WriteLine(
                        new string(
                            ' ',
                            maxNameLength
                                - opt.Syntax.Length
                                - aliasesLength
                                - placeholder.Length
                                + defaultPadding
                        ) + opt.Description
                    );
                    Console.Write(new string(' ', (level + 3) * 3) + "Values: ");
                    var values = Enum.GetNames(type);
                    WriteColor(string.Join(", ", values), ConsoleColor.Green);
                }
                else
                    Console.WriteLine(
                        new string(
                            ' ',
                            maxNameLength - opt.Syntax.Length - aliasesLength + defaultPadding
                        ) + opt.Description
                    );
            }
        }

        private static void DrawLine(char symbol, int length) =>
            Console.WriteLine(new string(symbol, length));
    }
}
