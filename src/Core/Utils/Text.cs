namespace Core.Utils
{
    public class Text
    {
        public static string CleanPath(string? Input) =>
            Input == null ? string.Empty : Input.Trim().Replace("\"", "");
    }
}
