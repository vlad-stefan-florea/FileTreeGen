namespace Core
{
    public class Metadata
    {
        public string dirName { get; set; } = string.Empty;
        public string dirPath { get; set; } = string.Empty;
        public string genDateTime { get; set; } = string.Empty;
        public Settings.PrivilegeLevel privileges { get; set; } = Settings.PrivilegeLevel.Standard;
    }
}
