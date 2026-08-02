namespace Core.Utils
{
    public class Text
    {
        public static string CleanPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;
            string cleaned = path.Trim().Replace("\"", "");
            if (cleaned.Length == 2 && cleaned.EndsWith(":")) // disk root reportType: "X:"
            {
                cleaned += Path.DirectorySeparatorChar;
            }
            if (cleaned.Length > 3)
            {
                // ensures that '\' is removed from the end (excluding disk/root reportType "X:\")
                // "C:\" - 3 chars - is not affected
                cleaned = cleaned.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                );
            }
            var dir = new DirectoryInfo(Path.GetFullPath(cleaned));
            string realPath = dir.FullName;
            return realPath;
        }

        public static string CleanFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "Untitled";

            char[] invalidChars = Path.GetInvalidFileNameChars();
            string cleanName = string.Join(
                "",
                fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)
            );
            return cleanName.Trim();
        }
    }
}
