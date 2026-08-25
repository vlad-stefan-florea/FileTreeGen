using static Core.Settings;

namespace Core
{
    public class GenFlags
    {
        // constructor
        public GenFlags() { }

        public string targetDir { get; set; } = string.Empty;
        public ReportType reportType { get; set; } = ReportType.HTML;
        public string outDir { get; set; } = downloadsDir;
        public BufferSize bufferSize { get; set; } = BufferSize.KB16;
        public ReportNameScheme reportNameScheme { get; set; } = ReportNameScheme.NameDate;
        public NodeLabel nodeLabel { get; set; } = NodeLabel.Name;
        public HashSet<string> extWhitelist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> extBlacklist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public bool dirsOnly { get; set; } = false;
        public bool filesOnly { get; set; } = false;
        public int maxDepth { get; set; } = int.MaxValue;
        public bool ignoreEmptyDirs { get; set; } = false;
        public bool ignoreSymlinks { get; set; } = false;
        public bool includeStatistics { get; set; } = true;
        public bool formatReport { get; set; } = true;
        public bool includeIcons { get; set; } = true;
        public bool autoOpenReport { get; set; } = false;
        public bool treeOnly { get; set; } = false;

        // folder paths
        private static string userprofile = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile
            ),
            downloadsDir = Path.Join(userprofile, "downloads");

        // copy
        public GenFlags(GenFlags Base)
        {
            targetDir = Base.targetDir;
            reportType = Base.reportType;
            outDir = Base.outDir;
            bufferSize = Base.bufferSize;
            reportNameScheme = Base.reportNameScheme;
            nodeLabel = Base.nodeLabel;

            extWhitelist = new HashSet<string>(Base.extWhitelist, StringComparer.OrdinalIgnoreCase);
            extBlacklist = new HashSet<string>(Base.extBlacklist, StringComparer.OrdinalIgnoreCase);

            dirsOnly = Base.dirsOnly;
            filesOnly = Base.filesOnly;
            maxDepth = Base.maxDepth;
            ignoreEmptyDirs = Base.ignoreEmptyDirs;
            ignoreSymlinks = Base.ignoreSymlinks;
            includeStatistics = Base.includeStatistics;
            formatReport = Base.formatReport;
            includeIcons = Base.includeIcons;
            autoOpenReport = Base.autoOpenReport;
            treeOnly = Base.treeOnly;
        }
    }
}
