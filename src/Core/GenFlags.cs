namespace Core
{
    public class GenFlags
    {
        public string targetDir { get; set; } = string.Empty;
        public Settings.OutputFormat format { get; set; } = Settings.OutputFormat.HTML;
        public string outPath { get; set; } = string.Empty;
        public Settings.BufferSize bufferSize { get; set; } = Settings.BufferSize.Medium;
        public List<string> extWhitelist { get; set; } = new();
        public List<string> extBlacklist { get; set; } = new();
        public int maxLevel { get; set; } = int.MaxValue;
        public bool ignoreEmptyDirs { get; set; } = false;
        public bool dirsOnly { get; set; } = false;
        public bool filesOnly { get; set; } = false;
        public bool noStatistics { get; set; } = false;
        public bool noFormatting { get; set; } = false;
        public bool noIcons { get; set; } = false;
        public bool autoOpenReport { get; set; } = false;
        public bool treeOnly { get; set; } = false;
    }
}
