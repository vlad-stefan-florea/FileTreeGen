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

        public static async Task<(CoreException, List<string>)> Run(string[] args)
        {
            List<string> errorsOut = new();
            CoreException exOut = new(ExitCode.Success, ExitMessages.Get(ExitCode.Success));
            // root
            RootCommand rootCmd = new("FileTreeGen - a simple and efficient file tree generator.");

            // target
            var data = CommandData.Arguments["target"];
            Argument<DirectoryInfo> targetArg = new(data.Name) { Description = data.Description };
            targetArg.AcceptExistingOnly();
            rootCmd.Arguments.Add(targetArg);

            #region OPTIONS

            #region Report

            // report type
            var repTypeOption = Commands.GenerateOption<ReportType>(
                "reportType",
                _flags.reportType
            );
            rootCmd.Options.Add(repTypeOption);
            // report name scheme
            var reportNameOption = Commands.GenerateOption("reportName", _flags.reportNameScheme);
            rootCmd.Options.Add(reportNameOption);
            // tree only
            var treeOnlyOption = Commands.GenerateOption("treeOnly", _flags.treeOnly);
            rootCmd.Options.Add(treeOnlyOption);

            #endregion
            #region Filtering

            // whitelist
            var whitelistOption = Commands.GenerateOption("whitelist", string.Empty);
            rootCmd.Options.Add(whitelistOption);
            // blacklist
            var blacklistOption = Commands.GenerateOption("blacklist", string.Empty);
            rootCmd.Options.Add(blacklistOption);
            // dirs only
            var dirsOnlyOption = Commands.GenerateOption("dirsOnly", _flags.dirsOnly);
            rootCmd.Options.Add(dirsOnlyOption);
            // files only
            var filesOnlyOption = Commands.GenerateOption("filesOnly", _flags.filesOnly);
            rootCmd.Options.Add(filesOnlyOption);

            #endregion
            #region Scan

            // ignore empty dirs
            var emptyDirsOption = Commands.GenerateOption(
                "ignoreEmptyDirs",
                _flags.ignoreEmptyDirs
            );
            rootCmd.Options.Add(emptyDirsOption);
            // ignore symlinks
            var symlinksOption = Commands.GenerateOption("ignoreSymlinks", _flags.ignoreSymlinks);
            rootCmd.Options.Add(symlinksOption);
            // buffer size
            var bufferOption = Commands.GenerateOption("buffer", _flags.bufferSize);
            rootCmd.Options.Add(bufferOption);
            // max depth
            var maxDepthOption = Commands.GenerateOption("maxDepth", _flags.maxDepth);
            maxDepthOption.Validators.Add(result =>
            {
                int value = result.GetValue(maxDepthOption);
                if (value < 1 || value > int.MaxValue)
                    result.AddError(
                        "The maximum scan depth can be any number from 1 to " + int.MaxValue + '.'
                    );
            });
            rootCmd.Options.Add(maxDepthOption);

            #endregion
            #region Presentation

            // node label
            var nodeLabelOption = Commands.GenerateOption("nodeLabel", _flags.nodeLabel);
            rootCmd.Options.Add(nodeLabelOption);
            // include statistics
            var noStatsOption = Commands.GenerateOption("noStats", !_flags.includeStatistics);
            rootCmd.Options.Add(noStatsOption);
            // report formatting
            var noFormattingOption = Commands.GenerateOption("noFormatting", !_flags.formatReport);
            rootCmd.Options.Add(noFormattingOption);
            // include icons
            var noIconsOption = Commands.GenerateOption("noIcons", !_flags.includeIcons);
            rootCmd.Options.Add(noIconsOption);

            #endregion
            #region Output

            // output dir
            var outOption = Commands.GenerateOption("outDir", new DirectoryInfo(_flags.outDir));
            outOption.AcceptExistingOnly();
            rootCmd.Options.Add(outOption);
            // auto open report
            var autoOpenOption = Commands.GenerateOption("autoOpen", _flags.autoOpenReport);
            rootCmd.Options.Add(autoOpenOption);
            #endregion

            #endregion

            // PARSING
            ParseResult parseResult = rootCmd.Parse(args);
            if (parseResult.Errors.Count > 0)
            {
                var errors = parseResult.Errors;
                foreach (var e in errors)
                    errorsOut.Add(e.Message);
                exOut = new CoreException(
                    ExitCode.InvalidArgument,
                    ExitMessages.Get(ExitCode.InvalidArgument)
                );
            }
            else
            {
                ApplyArgs(parseResult);
                try
                {
                    ValidateFlags();
                    await Generate();
                }
                catch (CoreException cex)
                {
                    return (cex, errorsOut);
                }
                catch (Exception ex)
                {
                    return (ExitMessages.TranslateOSException(ex), errorsOut);
                }
                if (_flags.autoOpenReport)
                    FileSystem.OpenPath(
                        ReportInfo.GeneratePath(
                            _flags.outDir,
                            _flags.targetDir,
                            _flags.reportType,
                            _flags.reportNameScheme
                        )
                    );
            }
            return (exOut, errorsOut);

            #region HELPERS
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
                    _flags.maxDepth = maxDepth;

                // IGNORE EMPTY DIRS
                if (result.GetValue(emptyDirsOption) is bool ignoreEmpty)
                    _flags.ignoreEmptyDirs = ignoreEmpty;

                // SYMLINKS
                if (result.GetValue(symlinksOption) is bool ignoreSymlinks)
                    _flags.ignoreSymlinks = ignoreSymlinks;

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
                    byWhitelist = _flags.extWhitelist.Count > 0,
                    byBlacklist = _flags.extBlacklist.Count > 0,
                    dontFormat = !_flags.formatReport,
                    treeOnly = _flags.treeOnly,
                    includeStats = _flags.includeStatistics;

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
                if (_flags.reportType == ReportType.HTML && dontFormat)
                    ThrowIncompatible("HTML Report", "No Report Formatting");
                if (dontFormat && _flags.includeIcons)
                    _flags.includeIcons = false; // no icons are used if 'no formatting' is active
                if (treeOnly && includeStats) // treeOnly has priority
                    _flags.includeStatistics = false;
            }
            void ThrowIncompatible(string arg1, string arg2)
            {
                errorsOut.Add($"'{arg1}' and '{arg2}' cannot be used simultaneously.");
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
