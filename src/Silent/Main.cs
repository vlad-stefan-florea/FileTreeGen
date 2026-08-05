using System.CommandLine;
using Core;
using Core.Utils;
using Core.Writers;
using static Core.Settings;

namespace Silent
{
    public static class Main
    {
        static GenFlags _flags = new GenFlags();

        public static async Task Run(string[] args)
        {
            // COMMANDS CONFIG

            // root
            RootCommand rootCmd = new("FileTreeGen - a simple and efficient file tree generator.");
            // target
            Argument<DirectoryInfo> targetArg = new("target")
            {
                Description = "The target directory to scan.",
            };
            rootCmd.Arguments.Add(targetArg);
            // report type
            Option<ReportType> repTypeOption = new("--type")
            {
                Aliases = { "-t" },
                Description = "The type of the generated report.",
            };
            // output dir
            Option<DirectoryInfo> outOption = new("--outdir")
            {
                Aliases = { "-o", "--out" },
                Description = "The directory where the report will be saved.",
            };
            rootCmd.Options.Add(outOption);
            // buffer size
            Option<BufferSize> bufferOption = new("--buffer")
            {
                Aliases = { "-bf" },
                Description = "The buffer size used while writing the report.",
            };
            rootCmd.Options.Add(bufferOption);
            // report name scheme
            Option<ReportNameScheme> reportNameOption = new("--reportname")
            {
                Aliases = { "-rn" },
                Description = "The report's naming scheme.",
            };
            // node label
            Option<NodeLabel> nodeLabelOption = new("--nodelabel")
            {
                Aliases = { "-nl" },
                Description = "The label type used for nodes.",
            };
            // whitelist
            Option<string> whitelistOption = new("--whitelist")
            {
                Aliases = { "-include" },
                Description = "The only extensions to include in the report.",
            };
            // blacklist
            Option<string> blacklistOption = new("--blacklist")
            {
                Aliases = { "-exclude" },
                Description = "The extensions to exclude from the report.",
            };
            // dirs only
            Option<bool> dirsOnlyOption = new("--dirsonly")
            {
                Aliases = { "-donly" },
                Description = "Include only directories in the report.",
            };
            // files only
            Option<bool> filesOnlyOption = new("--filesonly")
            {
                Aliases = { "-fonly" },
                Description = "Include only files in the report.",
            };
            // max depth
            Option<int> maxDepthOption = new("--maxdepth")
            {
                Aliases = { "--depth" },
                Description = "The maximum search depth.",
            };
            // ignore empty dirs
            Option<bool> emptyDirsOption = new("--ignoreemptydirs")
            {
                Aliases = { "-ied" },
                Description = "Include only files in the report.",
            };
            // include statistics
            Option<bool> excludeStatsOption = new("--excludestats")
            // includeStatistics = !value(excludeStatsOption)
            {
                Aliases = { "--nostats" },
                Description = "Do not include statistics in the generated report.",
            };
            // report formatting
            Option<bool> noFormattingOption = new("--noformatting")
            // formatReport = !value(noFormattingOption)
            {
                Aliases = { "-nf" },
                Description = "Do not use type specific formatting for the generated report.",
            };
            // include icons
            Option<bool> noIconsOption = new("--noicons")
            // includeIcons = !value(noIconsOption)
            {
                Aliases = { "-ni" },
                Description = "Do not use file type icons in the generated report.",
            };
            // auto open report
            Option<bool> autoOpenOption = new("--autoopen")
            {
                Aliases = { "--open", "-ao" },
                Description = "Automatically open the report after generation.",
            };
            // tree only
            Option<bool> treeOnlyOption = new("--treeonly")
            {
                Aliases = { "-t" },
                Description = "The report includes just the file tree.",
            };

            // PARSING
            ParseResult result = rootCmd.Parse(args);
            ApplyArgs(result);

            // helpers
            void ApplyArgs(ParseResult result)
            {
                // GENERAL VARIABLES
                string stringInput = string.Empty;
                bool boolInput = false;
                DirectoryInfo dirInfo;

                // TARGET DIR
                DirectoryInfo targetInfo = result.GetValue(targetArg);
                if (targetInfo != null)
                    _flags.targetDir = targetInfo.FullName;

                // OTHER IMPORTANT FLAGS
                bool onlyFiles = _flags.filesOnly,
                    onlyDirs = _flags.dirsOnly,
                    byWhitelist = _flags.extWhitelist.Any(),
                    byBlacklist = _flags.extBlacklist.Any();

                //FILTERING
                if ((byWhitelist && byBlacklist) || (onlyFiles && onlyDirs))
                    ThrowIncompatible();
            }
            void ThrowIncompatible() =>
                throw new CoreException(
                    ExitCode.IncompatibleArguments,
                    ExitMessages.Get(ExitCode.IncompatibleArguments)
                );
        }

        private static async Task Generate()
        {
            switch (_flags.reportType)
            {
                case ReportType.HTML:
                    HtmlWriter hmtlWriter = new HtmlWriter(_flags);
                    Task hmtlWrite = hmtlWriter.WriteAsync();
                    await hmtlWrite;
                    break;

                case ReportType.Markdown:
                    MarkdownWriter mdWriter = new MarkdownWriter(_flags);
                    Task mdWrite = mdWriter.WriteAsync();
                    await mdWrite;
                    break;

                case ReportType.Text:
                    Core.Writers.TextWriter txtWriter = new Core.Writers.TextWriter(_flags);
                    Task txtWrite = txtWriter.WriteAsync();
                    await txtWrite;
                    break;

                default:
                    break;
            }
            string outPath = ReportInfo.GeneratePath(
                _flags.outDir,
                _flags.targetDir,
                _flags.reportType,
                _flags.reportNameScheme
            );
            if (_flags.autoOpenReport)
                FileSystem.OpenPath(outPath);
        }
    }
}
