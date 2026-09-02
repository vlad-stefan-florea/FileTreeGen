using static CLI.CommandData;
using static CLI.Commands;
using static TUI.Display;

namespace CLI
{
    internal class HelpMenu
    {
        public static void DisplayHelp()
        {
            int defaultPadding = 10;
            // root command
            Console.WriteLine(RootCmd.Root.Description);
            WriteColor("USAGE", ConsoleColor.Cyan);
            WriteColor("   " + RootCmd.Root.Name, ConsoleColor.Yellow, newLine: false);
            WriteColor(" <subcommand> [arguments] [options]", ConsoleColor.Gray);
            Console.WriteLine();

            // subcommands
            WriteColor("SUBCOMMANDS", ConsoleColor.Cyan);
            int maxNameLength = RootCmd.SubCommands.Values.Max(x =>
                x.Name.Length + x.UsageArgs.Length + 1
            );
            foreach (var subcmd in RootCmd.SubCommands.Values)
            {
                WriteColor("   " + subcmd.Name + ' ', ConsoleColor.Yellow, newLine: false);
                WriteColor(subcmd.UsageArgs, ConsoleColor.Gray, newLine: false);
                Console.WriteLine(
                    new string(
                        ' ',
                        maxNameLength
                            - subcmd.Name.Length
                            - subcmd.UsageArgs.Length
                            - 1
                            + defaultPadding
                    ) + subcmd.Description
                );
            }

            #region SCAN_SUBCMD
            Console.WriteLine();
            WriteColor("SCAN SUBCOMMAND USAGE:", ConsoleColor.Cyan);
            WriteColor(
                "   " + RootCmd.SubCommands["scan"].Name + ' ',
                ConsoleColor.Yellow,
                newLine: false
            );
            WriteColor(RootCmd.SubCommands["scan"].UsageArgs, ConsoleColor.Gray);
            Console.WriteLine();
            // arguments
            WriteColor("ARGUMENTS:", ConsoleColor.Cyan);
            maxNameLength = ScanCmd.Arguments.Values.Max(x => x.Name.Length);
            foreach (var arg in ScanCmd.Arguments.Values.OrderBy(x => x.Name))
            {
                WriteColor("   " + arg.Name, ConsoleColor.Magenta, newLine: false);
                Console.WriteLine(
                    new string(' ', maxNameLength - arg.Name.Length + defaultPadding)
                        + arg.Description
                );
            }
            // options
            Console.WriteLine();
            WriteColor("OPTIONS:", ConsoleColor.Cyan);

            maxNameLength = ScanCmd.Options.Values.Max(x =>
                x.Name.Length + (x.Aliases != null ? x.Aliases.Sum(alias => alias.Length + 2) : 0)
            );
            foreach (var opt in ScanCmd.Options.Values.OrderBy(x => x.Name))
            {
                int aliasesLength = 0;
                var type = GetFlagType(opt.FlagName);
                WriteColor("   " + opt.Name, ConsoleColor.Magenta, newLine: false);
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
                                - opt.Name.Length
                                - aliasesLength
                                - placeholder.Length
                                + defaultPadding
                        ) + opt.Description
                    );
                    Console.Write("      Values: ");
                    var values = Enum.GetNames(type);
                    for (int i = 0; i < values.Length - 1; i++)
                    {
                        WriteColor(values[i], ConsoleColor.Green, newLine: false);
                        Console.Write(", ");
                    }
                    WriteColor(values[values.Length - 1], ConsoleColor.Green, newLine: false);
                    Console.WriteLine();
                }
                else
                    Console.WriteLine(
                        new string(
                            ' ',
                            maxNameLength - opt.Name.Length - aliasesLength + defaultPadding
                        ) + opt.Description
                    );
            }
            #endregion
        }
    }
}
