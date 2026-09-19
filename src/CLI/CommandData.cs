using Core;
using static CLI.Types;

namespace CLI
{
    internal class CommandData
    {
        /* place the Command() component at the end of each class,
         * so all the declared fields are initialized before the
         * main command is built
         */
        public static class Scan
        {
            public static List<Argument> Arguments =
            [
                new Argument("<target>", "The target directory to scan."),
            ];
            private static List<Option> Options =
            [
                new Option(
                    Name: "reportType",
                    Description: "The type of the generated report.",
                    Syntax: "--type",
                    FlagName: nameof(GenFlags.reportType),
                    Aliases: ["-t"]
                ),
                new Option(
                    Name: "outDir",
                    Description: "The directory where the report will be saved.",
                    Syntax: "--out-dir",
                    FlagName: nameof(GenFlags.outDir),
                    Aliases: ["-o"]
                ),
                new Option(
                    Name: "buffer",
                    Description: "The buffer size used while writing the report.",
                    Syntax: "--buffer",
                    FlagName: nameof(GenFlags.bufferSize)
                ),
                new Option(
                    Name: "reportName",
                    Description: "The report's naming scheme.",
                    Syntax: "--report-name",
                    FlagName: nameof(GenFlags.reportNameScheme)
                ),
                new Option(
                    Name: "nodeLabel",
                    Description: "The label type used for nodes.",
                    Syntax: "--node-label",
                    FlagName: nameof(GenFlags.nodeLabel)
                ),
                new Option(
                    Name: "whitelist",
                    Description: "The only extensions to include in the report.",
                    Syntax: "--whitelist",
                    FlagName: nameof(GenFlags.extWhitelist),
                    Aliases: ["--include"]
                ),
                new Option(
                    Name: "blacklist",
                    Description: "The extensions to exclude from the report.",
                    Syntax: "--blacklist",
                    FlagName: nameof(GenFlags.extBlacklist),
                    Aliases: ["--exclude"]
                ),
                new Option(
                    Name: "dirsOnly",
                    Description: "Include only directories in the report.",
                    Syntax: "--dirs-only",
                    FlagName: nameof(GenFlags.dirsOnly)
                ),
                new Option(
                    Name: "filesOnly",
                    Description: "Include only files in the report.",
                    Syntax: "--files-only",
                    FlagName: nameof(GenFlags.filesOnly)
                ),
                new Option(
                    Name: "maxDepth",
                    Description: "The maximum scan depth.",
                    Syntax: "--max-depth",
                    FlagName: nameof(GenFlags.maxDepth)
                ),
                new Option(
                    Name: "ignoreEmptyDirs",
                    Description: "Exclude empty directories from the report.",
                    Syntax: "--no-empty-dirs",
                    FlagName: nameof(GenFlags.ignoreEmptyDirs)
                ),
                new Option(
                    Name: "ignoreSymlinks",
                    Description: "Exclude symlink entries from the report.",
                    Syntax: "--no-symlinks",
                    FlagName: nameof(GenFlags.ignoreSymlinks)
                ),
                new Option(
                    Name: "noStats",
                    Description: "Do not include statistics in the generated report.",
                    Syntax: "--no-stats",
                    FlagName: nameof(GenFlags.includeStatistics)
                ),
                new Option(
                    Name: "noFormatting",
                    Description: "Do not use type specific formatting for the generated report.",
                    Syntax: "--no-formatting",
                    FlagName: nameof(GenFlags.formatReport)
                ),
                new Option(
                    Name: "noIcons",
                    Description: "Do not use file type icons in the generated report.",
                    Syntax: "--no-icons",
                    FlagName: nameof(GenFlags.includeIcons)
                ),
                new Option(
                    Name: "autoOpen",
                    Description: "Automatically open the report after generation.",
                    Syntax: "--auto-open",
                    FlagName: nameof(GenFlags.autoOpenReport),
                    Aliases: ["--open"]
                ),
                new Option(
                    Name: "treeOnly",
                    Description: "The report includes just the file tree.",
                    Syntax: "--tree-only",
                    FlagName: nameof(GenFlags.treeOnly)
                ),
                new Option(
                    Name: "dirBlacklist",
                    Description: "The directory names to exclude from the report.",
                    Syntax: "--filter-dirs",
                    FlagName: nameof(GenFlags.dirBlacklist)
                ),
            ];
            public static Command Cmd = new(
                Name: "Scan",
                Description: "Scans the target directory and generates a report.",
                Syntax: "scan",
                Arguments:
                [
                    new("<target>", "The directory to scan."),
                    new("[options]", "The command's options."),
                ],
                Options: Options
            );
        }

        public static class App
        {
            public static List<Command> SubCmds =
            [
                new Command(
                    Name: "CleanCrashDump",
                    Description: "Cleans FileTreeGen's crash dump folder.",
                    Syntax: "clean-crash-dump"
                ),
            ];
            public static Command Cmd = new(
                Name: "App",
                Description: "Provides access to app utilities.",
                Syntax: "app",
                Arguments: [new("<subcommand>", "Any utility subcommand.")],
                Subcommands: SubCmds
            );
        }

        public static class Root
        {
            private static List<Option> GlobalOptions =
            [
                new Option(
                    Name: "Verbosity",
                    Description: "The verbosity level of the output messages.",
                    Syntax: "--verbosity",
                    FlagName: nameof(Settings.Verbosity),
                    Aliases: ["-v"]
                ),
            ];
            public static Command Cmd = new(
                Name: "FileTreeGen",
                Description: "A simple and efficient file tree generator.",
                Syntax: "filetreegen",
                Arguments:
                [
                    new("[global options]", "Globally available options."),
                    new("<subcommand>", "Any FileTreeGen subcommand."),
                ],
                Subcommands: [Scan.Cmd, App.Cmd],
                Options: GlobalOptions
            );
        }
    }
}
