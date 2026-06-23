namespace Core.Utils
{
    public class Text
    {
        public static string CleanPath(string? path)
        {
            if (path == null)
                return string.Empty;
            string cleaned = path.Trim().Replace("\"", "");
            if (cleaned.Length == 2 && cleaned.EndsWith(":")) // disk root format: "X:"
            {
                cleaned += Path.DirectorySeparatorChar;
            }
            if (cleaned.Length > 3)
            {
                // ensures that '\' is removed from the end (excluding disk/root format "X:\")
                // "C:\" - 3 chars - is not affected
                cleaned = cleaned.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                );
            }
            return cleaned;
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
