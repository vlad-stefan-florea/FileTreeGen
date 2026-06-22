namespace Core
{
    public class Metadata
    {
        public string dirName { get; set; } = string.Empty;
        public string dirPath { get; set; } = string.Empty;
        public string genDateTime { get; set; } = string.Empty;
        public Settings.Privileges accessLevel { get; set; } = Settings.Privileges.Standard;
        public Dictionary<string, int> Extensions { get; set; } = new();
    }
}
