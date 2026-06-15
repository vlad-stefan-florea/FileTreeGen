namespace Core
{
    public class Settings
    {
        public enum OutputFormat
        {
            HTML,
            Markdown,
            Text,
        }

        public enum BufferSize
        {
            Small = 1024, // 1 KB
            Medium = 1024 * 4, // 4 KB (default)
            Large = 1024 * 8, // 8 KB
            ExtraLarge = 1024 * 16, // 16 KB
        }

        public enum AdvancedOptions
        {
            Back,
            Output_Directory,
            Buffer_Size,
            Extensions_Whitelist,
            Extensions_Blacklist,
        }
    }
}
