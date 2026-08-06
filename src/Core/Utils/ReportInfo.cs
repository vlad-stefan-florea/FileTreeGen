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

        private static string GetExtension(ReportType outputType) =>
            outputType switch
            {
                ReportType.HTML => ".html",
                ReportType.Markdown => ".md",
                ReportType.Text => ".txt",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(outputType),
                    $"Unsupported output reportType: {outputType}"
                ),
            };

        public static string GeneratePath(
            string outputDir,
            string targetDirPath,
            ReportType reportType,
            ReportNameScheme scheme
        ) =>
            Path.Join(
                Text.CleanPath(outputDir),
                GenerateName(targetDirPath)
                    + (
                        scheme switch
                        {
                            ReportNameScheme.NameDate => "-"
                                + Calendar.GetDateReversed().Replace("-", ""),
                            ReportNameScheme.NameDateTime => "-"
                                + Calendar.GetDateReversed().Replace("-", "")
                                + "-"
                                + Calendar.GetTime().Replace(":", ""),
                            _ => null, // .NameOnly included
                        }
                    )
                    + $"-{AppInfo.AppName}"
                    + GetExtension(reportType)
            );
    }
}
