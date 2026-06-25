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
            Big = 1024 * 8, // 8 KB
            Large = 1024 * 16, // 16 KB
            Extra_Large = 1024 * 32, // 32 KB
            High_Performance = 1024 * 64, // 64 KB
        }

        public enum AdvancedOptions
        {
            Auto_Open_Report,
            Buffer_Size,
            Directories_Only,
            Extensions_Blacklist,
            Extensions_Whitelist,
            Files_Only,
            Ignore_Empty_Directories,
            Max_Search_Depth,
            No_Statistics,
            No_Report_Formatting,
            No_Icons,
            Output_Directory,
        }

        public enum Privileges
        {
            Administrator,
            Standard,
        }

        public enum ByteScales
        {
            Bytes,
            KB,
            MB,
            GB,
            TB,
        }
    }
}
