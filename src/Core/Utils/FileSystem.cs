namespace Core.Utils
{
    public class FileSystem
    {
        public static bool IsReparsePoint(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
            {
                if ((dirInfo.Attributes & FileAttributes.ReparsePoint) != 0)
                    return true;
            }
            return false;
        }

        public static string ComputeSize(long sizeBytes)
        {
            double size = sizeBytes;
            int scale = 0;
            while (size >= 1024 && scale < Enum.GetValues<Settings.ByteScales>().Length - 1)
            {
                size /= 1024;
                scale++;
            }
            return $"{size:0.##} {(Settings.ByteScales)scale}";
        }

        public static void OpenPath(string path) =>
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo { FileName = path, UseShellExecute = true }
            );
    }
}
