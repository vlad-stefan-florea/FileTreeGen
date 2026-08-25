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
            KB4 = 4 * 1024, // 4 KB (default)
            KB16 = 16 * 1024, // 16 KB
            KB64 = 64 * 1024, // 64 KB
            KB256 = 256 * 1024, // 256 KB
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

        public enum Verbosity
        {
            Quiet,
            Normal,
            Verbose,
        }
    }
}
