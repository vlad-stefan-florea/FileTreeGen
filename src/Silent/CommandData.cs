using Core;
using Core.Utils;
using static Silent.Commands;

namespace Silent
{
    public class CommandData
    {
        public static Dictionary<string, Cmd> Arguments = new()
        {
            ["target"] = new Cmd(
                "target",
                "The target directory to scan.",
                nameof(GenFlags.targetDir)
            ),
        };
        public static Dictionary<string, Cmd> Options = new()
        {
            ["reportType"] = new Cmd(
                "--type",
                "The type of the generated report.",
                nameof(GenFlags.reportType),
                ["-t"]
            ),
            ["outDir"] = new Cmd(
                "--out-dir",
                "The directory where the report will be saved.",
                nameof(GenFlags.outDir),
                ["-o"]
            ),
            ["buffer"] = new Cmd(
                "--buffer",
                "The buffer size used while writing the report.",
                nameof(GenFlags.bufferSize)
            ),
            ["reportName"] = new Cmd(
                "--report-name",
                "The report's naming scheme.",
                nameof(GenFlags.reportNameScheme)
            ),
            ["nodeLabel"] = new Cmd(
                "--node-label",
                "The label type used for nodes.",
                nameof(GenFlags.nodeLabel)
            ),
            ["whitelist"] = new Cmd(
                "--whitelist",
                "The only extensions to include in the report.",
                nameof(GenFlags.extWhitelist),
                ["--include"]
            ),
            ["blacklist"] = new Cmd(
                "--blacklist",
                "The extensions to exclude from the report.",
                nameof(GenFlags.extBlacklist),
                ["--exclude"]
            ),
            ["dirsOnly"] = new Cmd(
                "--dirs-only",
                "Include only directories in the report.",
                nameof(GenFlags.dirsOnly)
            ),
            ["filesOnly"] = new Cmd(
                "--files-only",
                "Include only files in the report.",
                nameof(GenFlags.filesOnly)
            ),
            ["maxDepth"] = new Cmd(
                "--max-depth",
                "The maximum scan depth.",
                nameof(GenFlags.maxDepth)
            ),
            ["ignoreEmptyDirs"] = new Cmd(
                "--ignore-empty-dirs",
                "Exclude empty directories from the report.",
                nameof(GenFlags.ignoreEmptyDirs)
            ),
            ["ignoreSymlinks"] = new Cmd(
                "--ignore-symlinks",
                "Exclude symlink entries from the report.",
                nameof(GenFlags.ignoreSymlinks)
            ),
            ["noStats"] = new Cmd(
                "--no-stats",
                "Do not include statistics in the generated report.",
                nameof(GenFlags.includeStatistics)
            ),
            ["noFormatting"] = new Cmd(
                "--no-formatting",
                "Do not use type specific formatting for the generated report.",
                nameof(GenFlags.formatReport)
            ),
            ["noIcons"] = new Cmd(
                "--no-icons",
                "Do not use file type icons in the generated report.",
                nameof(GenFlags.includeIcons)
            ),
            ["autoOpen"] = new Cmd(
                "--auto-open",
                "Automatically open the report after generation.",
                nameof(GenFlags.autoOpenReport),
                ["--open"]
            ),
            ["treeOnly"] = new Cmd(
                "--tree-only",
                "The report includes just the file tree.",
                nameof(GenFlags.treeOnly)
            ),
        };
    }
}
