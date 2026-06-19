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
            Output_Directory,
            Buffer_Size,
            Extensions_Whitelist,
            Extensions_Blacklist,
            Max_Search_Depth,
            Ignore_Empty_Directories,
            Directories_Only,
            Files_Only,
            No_Statistics,
            No_Report_Formatting,
            No_Icons,
            Auto_Open_Report,
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
