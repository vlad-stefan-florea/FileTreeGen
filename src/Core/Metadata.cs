namespace Core
{
    public class Metadata
    {
        public string dirName { get; set; } = string.Empty;
        public string dirPath { get; set; } = string.Empty;
        public string genDateTime { get; set; } = string.Empty;
        public string genTimespan { get; set; } = string.Empty;
        public Settings.Privileges priviliges { get; set; } = Settings.Privileges.Standard;
        public int folders { get; set; } = 0;
        public int files { get; set; } = 0;
        public int skippedFolders { get; set; } = 0;
        public long totalSizeBytes { get; set; } = 0;
    }
}
