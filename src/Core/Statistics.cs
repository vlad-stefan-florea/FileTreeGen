namespace Core
{
    public class Statistics
    {
        public string genTimespan { get; set; } = string.Empty;
        public int files { get; set; } = 0;
        public int skippedFiles { get; set; } = 0;
        public int folders { get; set; } = 0;
        public int skippedFolders { get; set; } = 0;
        public int symlinks { get; set; } = 0;
        public long totalSizeBytes { get; set; } = 0;
    }
}
