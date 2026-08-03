namespace Core
{
    public class Settings
    {
        public enum ReportType
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

        public enum Privileges
        {
            Administrator,
            Standard,
        }

        public enum NodeLabel
        {
            Name,
            Full_Path,
            Relative_Path,
        }

        public enum ReportNameScheme
        {
            Name_Only, // <folder name>-FileTreeGen
            Name_Date, // <folder name>-yyyyMMdd-FileTreeGen
            Name_Date_Time, // <folder name>-yyyyMMdd-HHmmss-FileTreeGen
        }

        public enum CliAdvancedOptions
        {
            Auto_Open_Report,
            Buffer_Size,
            Directories_Only,
            Extensions_Blacklist,
            Extensions_Whitelist,
            Files_Only,
            Format_Report,
            Ignore_Empty_Directories,
            Include_Icons,
            Include_Statistics,
            Max_Search_Depth,
            Node_Label_Scheme,
            Output_Directory,
            Report_Name_Scheme,
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
