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
            ExtraLarge = 1024 * 32, // 32 KB
            HighPerformance = 1024 * 64, // 64 KB
        }

        public enum PrivilegeLevel
        {
            Standard,
            Elevated,
            Unknown,
        }

        public enum NodeLabel
        {
            Name,
            FullPath,
            RelativePath,
        }

        public enum ReportNameScheme
        {
            NameOnly, // <folder name>-FileTreeGen
            NameDate, // <folder name>-yyyyMMdd-FileTreeGen
            NameDateTime, // <folder name>-yyyyMMdd-HHmmss-FileTreeGen
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
            Ignore_SymLinks,
            Include_Icons,
            Include_Statistics,
            Max_Search_Depth,
            Node_Label_Scheme,
            Output_Directory,
            Report_Name_Scheme,
            Tree_Only,
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
