using static Core.Settings;

namespace Core
{
    public class GenFlags
    {
        // constructor
        public GenFlags() { }

        // flags
        #region REPORT
        public ReportType reportType { get; set; } = ReportType.HTML;
        public ReportNameScheme reportNameScheme { get; set; } = ReportNameScheme.NameDate;
        public bool treeOnly { get; set; } = false;

        #endregion

        #region FILTERING
        public HashSet<string> extWhitelist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> extBlacklist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> dirBlacklist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public bool dirsOnly { get; set; } = false;
        public bool filesOnly { get; set; } = false;

        #endregion

        #region SCAN
        public string targetDir { get; set; } = string.Empty;
        public BufferSize bufferSize { get; set; } = BufferSize.KB16;
        public int maxDepth { get; set; } = int.MaxValue;
        public bool ignoreEmptyDirs { get; set; } = false;
        public bool ignoreSymlinks { get; set; } = false;
        public bool includeStatistics { get; set; } = true;

        #endregion

        #region PRESENTATION
        public NodeLabel nodeLabel { get; set; } = NodeLabel.Name;
        public bool formatReport { get; set; } = true;
        public bool includeIcons { get; set; } = true;

        #endregion

        #region OUTPUT
        public string outDir { get; set; } = downloadsDir;
        public bool autoOpenReport { get; set; } = false;

        #endregion

        // folder paths
        private static string userprofile = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile
            ),
            downloadsDir = Path.Join(userprofile, "downloads");

        // copy
        public GenFlags(GenFlags Base)
        {
            // report
            reportType = Base.reportType;
            reportNameScheme = Base.reportNameScheme;
            treeOnly = Base.treeOnly;
            // filtering
            extWhitelist = new HashSet<string>(Base.extWhitelist, StringComparer.OrdinalIgnoreCase);
            extBlacklist = new HashSet<string>(Base.extBlacklist, StringComparer.OrdinalIgnoreCase);
            dirBlacklist = new HashSet<string>(Base.dirBlacklist, StringComparer.OrdinalIgnoreCase);
            dirsOnly = Base.dirsOnly;
            filesOnly = Base.filesOnly;
            // scan
            targetDir = Base.targetDir;
            bufferSize = Base.bufferSize;
            maxDepth = Base.maxDepth;
            ignoreEmptyDirs = Base.ignoreEmptyDirs;
            ignoreSymlinks = Base.ignoreSymlinks;
            includeStatistics = Base.includeStatistics;
            // presentation
            formatReport = Base.formatReport;
            includeIcons = Base.includeIcons;
            nodeLabel = Base.nodeLabel;
            // output
            outDir = Base.outDir;
            autoOpenReport = Base.autoOpenReport;
        }
    }
}
