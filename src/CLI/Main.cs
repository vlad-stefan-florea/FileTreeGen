using Core;
using Core.Utils;
using Core.Writers;
using static Core.Settings;

namespace CLI
{
    public class Main
    {
        static GenFlags _flags = new GenFlags();

        public static async Task<(CoreException, List<string>, Verbosity)> Run(string[] args)
        {
            List<string> messagesOut = new();
            CoreException exOut = new(ExitCode.Success, ExitMessages.Get(ExitCode.Success));
            Verbosity verbosity = Verbosity.Normal;

            // PARSING
            string[] helpAliases = ["--help", "-h", "-?", "/h", "/?"];
            foreach (string item in helpAliases)
            {
                if (args.Contains(item))
                {
                    HelpMenu.DisplayHelp();
                    return (exOut, messagesOut, verbosity);
                }
            }
            if (args.Contains("--version"))
            {
                Console.WriteLine(AppInfo.Version);
                return (exOut, messagesOut, verbosity);
            }

            return (exOut, messagesOut, verbosity);

            #region HELPERS

            void ValidateFlags(GenFlags flags)
            {
                bool onlyFiles = flags.filesOnly,
                    onlyDirs = flags.dirsOnly,
                    ignoreEmptyDirs = flags.ignoreEmptyDirs,
                    byWhitelist = flags.extWhitelist.Count > 0,
                    byBlacklist = flags.extBlacklist.Count > 0,
                    dontFormat = !flags.formatReport,
                    treeOnly = flags.treeOnly,
                    includeStats = flags.includeStatistics;

                if (byWhitelist && byBlacklist)
                    ThrowIncompatible("Whitelist", "Blacklist");
                if (byWhitelist && onlyDirs)
                    ThrowIncompatible("Whitelist", "Dirs Only");
                if (byBlacklist && onlyDirs)
                    ThrowIncompatible("Blacklist", "Dirs Only");
                if (onlyFiles && onlyDirs)
                    ThrowIncompatible("Files Only", "Dirs Only");
                if (onlyFiles && ignoreEmptyDirs)
                    ThrowIncompatible("Files Only", "Ignore Empty Dirs");
                if (flags.reportType == ReportType.HTML && dontFormat)
                    ThrowIncompatible("HTML Report", "No Report Formatting");
                if (dontFormat && flags.includeIcons)
                    flags.includeIcons = false; // no icons are used if 'no formatting' is active
                if (treeOnly && includeStats) // treeOnly has priority
                    flags.includeStatistics = false;
                if (treeOnly && flags.reportType == ReportType.HTML)
                    ThrowIncompatible("HTML Report", "Tree Only");
            }
            void ThrowIncompatible(string arg1, string arg2)
            {
                messagesOut.Add($"'{arg1}' and '{arg2}' cannot be used at the same time.");
                throw new CoreException(
                    ExitCode.IncompatibleArguments,
                    ExitMessages.Get(ExitCode.IncompatibleArguments)
                );
            }
            #endregion
        }

        private static async Task Generate()
        {
            switch (_flags.reportType)
            {
                case ReportType.HTML:
                    HtmlWriter hmtlWriter = new HtmlWriter(_flags);
                    await hmtlWriter.WriteAsync();
                    break;

                case ReportType.Markdown:
                    MarkdownWriter mdWriter = new MarkdownWriter(_flags);
                    await mdWriter.WriteAsync();
                    break;

                case ReportType.Text:
                    Core.Writers.TextWriter txtWriter = new Core.Writers.TextWriter(_flags);
                    await txtWriter.WriteAsync();
                    break;

                default:
                    break;
            }
        }
    }
}
