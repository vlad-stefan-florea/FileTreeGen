using static Core.Settings;

namespace Core.Utils
{
    public class ReportInfo
    {
        public static string GenerateName(string targetPath) =>
            Utils.Text.CleanFileName(Path.GetFileName(Utils.Text.CleanPath(targetPath)));

        private static string GetExtension(OutputFormat outputType) =>
            outputType switch
            {
                OutputFormat.HTML => ".html",
                OutputFormat.Markdown => ".md",
                OutputFormat.Text => ".txt",
                _ => throw new ArgumentOutOfRangeException(
                    nameof(outputType),
                    $"Unsupported output format: {outputType}"
                ),
            };

        public static string GeneratePath(
            string outputDir,
            string targetDir,
            OutputFormat Format
        ) =>
            Path.Join(
                Utils.Text.CleanPath(outputDir),
                GenerateName(targetDir)
                    + "-"
                    + Calendar.GetSimpleDate()
                    + "-FileTreeGen"
                    + GetExtension(Format)
            );
    }
}
