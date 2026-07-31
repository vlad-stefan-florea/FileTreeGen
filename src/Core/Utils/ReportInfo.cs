using static Core.Settings;

namespace Core.Utils
{
    public class ReportInfo
    {
        public static string GenerateName(string targetPath)
        {
            string cleanedPath = Text.CleanPath(targetPath);
            var root = Path.GetPathRoot(cleanedPath);
            if (
                root != null
                && string.Equals(
                    cleanedPath.TrimEnd(Path.DirectorySeparatorChar),
                    root.TrimEnd(Path.DirectorySeparatorChar),
                    StringComparison.OrdinalIgnoreCase
                )
            )
                return "Drive_" + root.TrimEnd(Path.DirectorySeparatorChar).Replace(":", "");
            return Text.CleanFileName(Path.GetFileName(cleanedPath)).Replace(" ", "_");
        }

        private static string GetExtension(OutputFormat outputType) =>
            outputType switch
            {
                OutputFormat.HTML => ".html",
                OutputFormat.Markdown => ".md",
                OutputFormat.Text => ".txt",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(outputType),
                    $"Unsupported output reportType: {outputType}"
                ),
            };

        public static string GeneratePath(
            string outputDir,
            string targetDir,
            OutputFormat Format
        ) =>
            Path.Join(
                Text.CleanPath(outputDir),
                GenerateName(targetDir)
                    + "-"
                    + Calendar.GetDateReversed()
                    + $"-{AppInfo.AppName}"
                    + GetExtension(Format)
            );
    }
}
