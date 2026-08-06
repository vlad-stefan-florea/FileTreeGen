using System.CommandLine;
using System.Security.Cryptography;
using Core;
using Core.Utils;
using Core.Writers;
using static CLI.Display;
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
            targetArg.AcceptExistingOnly();
            rootCmd.Arguments.Add(targetArg);

            // report type
            Option<ReportType> repTypeOption = new("--type")
            {
                Aliases = { "-t" },
                Description = "The type of the generated report.",
                DefaultValueFactory = parseResult => _flags.reportType,
            };
            rootCmd.Options.Add(repTypeOption);

            // output dir
            Option<DirectoryInfo> outOption = new("--out-dir")
            {
                Aliases = { "-o" },
                Description = "The directory where the report will be saved.",
                DefaultValueFactory = parseResult => new DirectoryInfo(_flags.outDir),
            };
            outOption.AcceptExistingOnly();
            rootCmd.Options.Add(outOption);

            // buffer size
            Option<BufferSize> bufferOption = new("--buffer")
            {
                Description = "The buffer size used while writing the report.",
                DefaultValueFactory = parseResult => _flags.bufferSize,
            };
            rootCmd.Options.Add(bufferOption);

            // report name scheme
            Option<ReportNameScheme> reportNameOption = new("--report-name")
            {
                Description = "The report's naming scheme.",
                DefaultValueFactory = parseResult => _flags.reportNameScheme,
            };
            rootCmd.Options.Add(reportNameOption);

            // node label
            Option<NodeLabel> nodeLabelOption = new("--node-label")
            {
                Description = "The label type used for nodes.",
                DefaultValueFactory = parseResult => _flags.nodeLabel,
            };
            rootCmd.Options.Add(nodeLabelOption);

            // whitelist
            Option<string> whitelistOption = new("--whitelist")
            {
                Aliases = { "--include" },
                Description = "The only extensions to include in the report.",
                DefaultValueFactory = parseResult => string.Empty,
            };
            rootCmd.Options.Add(whitelistOption);

            // blacklist
            Option<string> blacklistOption = new("--blacklist")
            {
                Aliases = { "--exclude" },
                Description = "The extensions to exclude from the report.",
                DefaultValueFactory = parseResult => string.Empty,
            };
            rootCmd.Options.Add(blacklistOption);

            // dirs only
            Option<bool> dirsOnlyOption = new("--dirs-only")
            {
                Description = "Include only directories in the report.",
                DefaultValueFactory = parseResult => _flags.dirsOnly,
            };
            rootCmd.Options.Add(dirsOnlyOption);

            // files only
            Option<bool> filesOnlyOption = new("--files-only")
            {
                Description = "Include only files in the report.",
                DefaultValueFactory = parseResult => _flags.filesOnly,
            };
            rootCmd.Options.Add(filesOnlyOption);

            // max depth
            Option<int> maxDepthOption = new("--max-depth")
            {
                Aliases = { "--depth" },
                Description = "The maximum search depth.",
                DefaultValueFactory = parseResult => _flags.maxDepth,
            };
            rootCmd.Options.Add(maxDepthOption);

            // ignore empty dirs
            Option<bool> emptyDirsOption = new("--ignore-empty-dirs")
            {
                Description = "Include only files in the report.",
                DefaultValueFactory = parseResult => _flags.ignoreEmptyDirs,
            };
            rootCmd.Options.Add(emptyDirsOption);

            // ignore symlinks
            Option<bool> symlinksOption = new("--ignore-symlinks")
            {
                Description = "Exclude symlink entries from the report.",
                DefaultValueFactory = parseResult => _flags.ignoreSymLinks,
            };
            rootCmd.Options.Add(symlinksOption);

            // include statistics
            Option<bool> noStatsOption = new("--no-stats")
            // includeStatistics = !value(excludeStatsOption)
            {
                Description = "Do not include statistics in the generated report.",
                DefaultValueFactory = parseResult => !_flags.includeStatistics,
            };
            rootCmd.Options.Add(noStatsOption);

            // report formatting
            Option<bool> noFormattingOption = new("--no-formatting")
            // formatReport = !value(noFormattingOption)
            {
                Description = "Do not use type specific formatting for the generated report.",
                DefaultValueFactory = parseResult => !_flags.formatReport,
            };
            rootCmd.Options.Add(noFormattingOption);

            // include icons
            Option<bool> noIconsOption = new("--no-icons")
            // includeIcons = !value(noIconsOption)
            {
                Description = "Do not use file type icons in the generated report.",
                DefaultValueFactory = parseResult => !_flags.includeIcons,
            };
            rootCmd.Options.Add(noIconsOption);

            // auto open report
            Option<bool> autoOpenOption = new("--auto-open")
            {
                Aliases = { "--open" },
                Description = "Automatically open the report after generation.",
                DefaultValueFactory = parseResult => _flags.autoOpenReport,
            };
            rootCmd.Options.Add(autoOpenOption);

            // tree only
            Option<bool> treeOnlyOption = new("--tree-only")
            {
                Description = "The report includes just the file tree.",
                DefaultValueFactory = parseResult => _flags.treeOnly,
            };
            rootCmd.Options.Add(treeOnlyOption);

            // PARSING
            rootCmd.SetAction(async result =>
            {
                try
                {
                    ApplyArgs(result);
                    ValidateFlags();
                    await Generate();
                    if (_flags.autoOpenReport)
                        FileSystem.OpenPath(
                            ReportInfo.GeneratePath(
                                _flags.outDir,
                                _flags.targetDir,
                                _flags.reportType,
                                _flags.reportNameScheme
                            )
                        );
                    return 0;
                }
                catch (CoreException ex)
                {
                    WriteMsg(ex.Message, MsgType.Error);
                    return (int)ex.Code;
                }
            });

            ParseResult parseResult = rootCmd.Parse(args);
            await parseResult.InvokeAsync();

            // HELPERS
            void ApplyArgs(ParseResult result)
            {
                // TARGET DIR
                if (result.GetValue(targetArg) is DirectoryInfo targetDir)
                    _flags.targetDir = targetDir.FullName;

                // REPORT TYPE
                if (result.GetValue(repTypeOption) is ReportType type)
                    _flags.reportType = type;

                // OUT DIR
                if (result.GetValue(outOption) is DirectoryInfo outDir)
                    _flags.outDir = outDir.FullName;

                // BUFFER SIZE
                if (result.GetValue(bufferOption) is BufferSize value)
                {
                    _flags.bufferSize = value;
                }

                // REPORT NAME SCHEME
                if (result.GetValue(reportNameOption) is ReportNameScheme scheme)
                    _flags.reportNameScheme = scheme;

                // NODE LABEL
                if (result.GetValue(nodeLabelOption) is NodeLabel label)
                    _flags.nodeLabel = label;

                // WHITELIST
                if (result.GetValue(whitelistOption) is string whutelist)
                    _flags.extWhitelist = ListParser.ParseExtensionList(whutelist);

                // BLACKLIST
                if (result.GetValue(blacklistOption) is string blacklist)
                    _flags.extBlacklist = ListParser.ParseExtensionList(blacklist);

                // DIRS ONLY
                if (result.GetValue(dirsOnlyOption) is bool dirsOnly)
                    _flags.dirsOnly = dirsOnly;

                // FILES ONLY
                if (result.GetValue(filesOnlyOption) is bool filesOnly)
                    _flags.filesOnly = filesOnly;

                // MAX DEPTH
                if (result.GetValue(maxDepthOption) is int maxDepth)
                {
                    if (maxDepth < 1 || maxDepth > int.MaxValue)
                        ThrowInvalid("max depth", "any value from 1 to " + int.MaxValue);
                    else
                        _flags.maxDepth = maxDepth;
                }

                // IGNORE EMPTY DIRS
                if (result.GetValue(emptyDirsOption) is bool ignoreEmpty)
                    _flags.ignoreEmptyDirs = ignoreEmpty;

                // SYMLINKS
                if (result.GetValue(symlinksOption) is bool ignoreSymlinks)
                    _flags.ignoreSymLinks = ignoreSymlinks;

                // NO STATISTICS
                if (result.GetValue(noStatsOption) is bool noStats)
                    _flags.includeStatistics = !noStats;

                // NO FORMATTING
                if (result.GetValue(noFormattingOption) is bool noFormatting)
                    _flags.formatReport = !noFormatting;

                // NO ICONS
                if (result.GetValue(noIconsOption) is bool noIcons)
                    _flags.includeIcons = !noIcons;

                // AUTO OPEN
                if (result.GetValue(autoOpenOption) is bool autoOpen)
                    _flags.autoOpenReport = autoOpen;

                // TREE ONLY
                if (result.GetValue(treeOnlyOption) is bool treeOnly)
                    _flags.treeOnly = treeOnly;
            }
            void ValidateFlags()
            {
                bool onlyFiles = _flags.filesOnly,
                    onlyDirs = _flags.dirsOnly,
                    ignoreEmptyDirs = _flags.ignoreEmptyDirs,
                    byWhitelist = _flags.extWhitelist.Any(),
                    byBlacklist = _flags.extBlacklist.Any(),
                    dontFormat = !_flags.formatReport;

                if (byWhitelist && byBlacklist)
                    ThrowIncompatible("filter by whitelist", "filter by blacklist");
                if (onlyFiles && onlyDirs)
                    ThrowIncompatible("files only", "dirs only");
                if (onlyFiles && ignoreEmptyDirs)
                    ThrowIncompatible("files only", "ignore empty dirs");
                if (_flags.reportType == ReportType.HTML && dontFormat)
                    ThrowIncompatible("HTML report type", "no report formatting");
            }
            void ThrowIncompatible(string arg1, string arg2) =>
                throw new CoreException(
                    ExitCode.IncompatibleArguments,
                    ExitMessages.Get(ExitCode.IncompatibleArguments) + $" ('{arg1}' & '{arg2}')"
                );
            void ThrowInvalid(string arg, string? msg) =>
                throw new CoreException(
                    ExitCode.InvalidArgument,
                    ExitMessages.Get(ExitCode.InvalidArgument) + $" ('{arg}')" + (msg ?? ": " + msg)
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
