namespace Core
{
    public class GenFlags
    {
        public string targetDir { get; set; } = string.Empty;
        public Settings.ReportType reportType { get; set; } = Settings.ReportType.HTML;
        public string outPath { get; set; } = string.Empty;
        public Settings.BufferSize bufferSize { get; set; } = Settings.BufferSize.Medium;
        public Settings.ReportNameScheme reportNameScheme { get; set; } =
            Settings.ReportNameScheme.Name_Date;
        public Settings.NodeLabel nodeLabel { get; set; } = Settings.NodeLabel.Name;
        public HashSet<string> extWhitelist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> extBlacklist { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public int maxLevel { get; set; } = int.MaxValue;
        public bool ignoreEmptyDirs { get; set; } = false;
        public bool dirsOnly { get; set; } = false;
        public bool filesOnly { get; set; } = false;
        public bool includeStatistics { get; set; } = true;
        public bool formatReport { get; set; } = true;
        public bool includeIcons { get; set; } = true;
        public bool autoOpenReport { get; set; } = false;
        public bool treeOnly { get; set; } = false;
    }
}
