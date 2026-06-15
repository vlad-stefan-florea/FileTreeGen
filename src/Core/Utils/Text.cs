namespace Core.Utils
{
    public class Text
    {
        public static string CleanPath(string? path) =>
            path == null
                ? string.Empty
                : path.Trim()
                    .Replace("\"", "")
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

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
