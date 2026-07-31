namespace Core
{
    public class GenFlags
    {
        public string targetDir { get; set; } = string.Empty;
        public Settings.OutputFormat reportType { get; set; } = Settings.OutputFormat.HTML;
        public string outPath { get; set; } = string.Empty;
        public Settings.BufferSize bufferSize { get; set; } = Settings.BufferSize.Medium;
        public HashSet<string> extWhitelist { get; set; } = new();
        public HashSet<string> extBlacklist { get; set; } = new();
        public int maxLevel { get; set; } = int.MaxValue;
        public bool ignoreEmptyDirs { get; set; } = false;
        public bool dirsOnly { get; set; } = false;
        public bool filesOnly { get; set; } = false;
        public bool includeStatistics { get; set; } = true;
        public bool formatReport { get; set; } = true;
        public bool includeIcons { get; set; } = true;
        public bool autoOpenReport { get; set; } = false;
        public bool treeOnly { get; set; } = false;
        public bool useFullPaths { get; set; } = false;
    }
}
