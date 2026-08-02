using System.Text;
using System.Text.Json;

namespace Core.Writers
{
    public sealed class HtmlWriter : BaseWriter
    {
        private static bool isFirstNode = true;
        private static GenFlags _flags = new();

        public HtmlWriter(GenFlags flags)
            : base(flags)
        {
            _flags = flags;
        }

        protected override async Task WriteHeaderAsync(StreamWriter writer) =>
            await writer.WriteAsync(Header());

        protected override async Task WriteFooterAsync(StreamWriter writer) =>
            await writer.WriteAsync(Footer());

        protected override async Task WriteMetadataAsync(StreamWriter writer) =>
            await writer.WriteAsync(MetadataPanel());

        protected override async Task WriteStatisticsAsync(StreamWriter writer) =>
            await writer.WriteAsync(StatsPanel());

        protected override async Task WriteNodeAsync(StreamWriter writer, Node node)
        {
            if (!isFirstNode)
                await writer.WriteAsync(",");
            isFirstNode = false;
            if (!string.IsNullOrEmpty(node.Path))
                node.Path = node.Path.Replace("\\", "/");
            var line = JsonSerializer.Serialize(node, AppJsonContext.Default.Node);
            await writer.WriteAsync(line);
        }

        private string Header() =>
            HtmlStruct.HtmlStart(Generator.metadata.dirName)
            + HtmlStruct.Styles()
            + "</head>"
            + "<body>"
            + "<div class=\"prow\">";

        private class HtmlStruct
        {
            internal static string HtmlStart(string dirName) =>
                $"<!doctype html><html lang=\"en\"><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><head><title>{Utils.ReportInfo.GenerateName(dirName) + " " + AppInfo.AppName + " report"}</title>";

            internal static string Styles()
            {
                StringBuilder htmlHeader = new();
                htmlHeader.Append("<style>");
                htmlHeader.Append(
                    Utils.EmbeddedReader.ReadEmbeddedResource("Core.HtmlTemplates.styles.css")
                );
                htmlHeader.Append("</style>");
                return htmlHeader.ToString().Replace("\n", "").Replace("\r", "");
            }

            internal static string RemainingBody() =>
                "</div><hr/>"
                + "<div id=\"treeContainer\"></div>"
                + $"<hr/><footer>Generated using <a href=\"{AppInfo.AppUrl}\" target=\"_blank\">{AppInfo.AppName}</a></footer>"
                + "<script>const nodeList=[";

            internal static string Scripts(string[] foundExtensions, string dirName)
            {
                StringBuilder html = new();

                html.Append(
                    HtmlTemplates.Icons.GenSvgCollectionHtml(foundExtensions, !_flags.includeIcons)
                );
                html.Append(
                    "<script>"
                        + $"const noIcons={(!_flags.includeIcons ? "true" : "false")};"
                        + $"const filesOnly={(_flags.filesOnly ? "true" : "false")};"
                        + "</script>"
                );
                html.Append($"<script>");
                html.Append(
                    Utils.EmbeddedReader.ReadEmbeddedResource("Core.HtmlTemplates.navbar.js")
                );
                html.Append("</script>");
                html.Append($"<script>");
                html.Append(
                    Utils.EmbeddedReader.ReadEmbeddedResource("Core.HtmlTemplates.treeLogic.js")
                );
                html.Append("</script>");
                return html.ToString()
                    .Replace("___dirName___", dirName)
                    .Replace("\n", "")
                    .Replace("\r", "");
            }
        }

        private string MetadataPanel()
        {
            StringBuilder html = new();
            string ariaLabel =
                "REPORT METADATA SECTION: "
                + $"TARGET DIRECTORY:{Generator.metadata.dirName} ;"
                + $"TARGET PATH: {Generator.metadata.dirPath} ;"
                + $"ACCESS LEVEL: {Generator.metadata.accessLevel} ;"
                + $"GENERATED AT: {Generator.metadata.genDateTime} ;";
            html.Append(
                $"<div class=\"p\" tabindex=\"0\" aria-label=\"{ariaLabel}\"><h4>REPORT METADATA</h4><ul>"
            );
            html.Append($"<li><b>TARGET DIRECTORY:</b> {Generator.metadata.dirName}</li>");
            html.Append($"<li><b>TARGET PATH:</b> {Generator.metadata.dirPath}</li>");
            html.Append($"<li><b>ACCESS LEVEL:</b> {Generator.metadata.accessLevel}</li>");
            html.Append($"<li><b>GENERATED AT:</b> {Generator.metadata.genDateTime}</li>");
            html.Append("</ul></div>");
            if (!_flags.includeStatistics)
                html.Append(HtmlStruct.RemainingBody());
            return html.ToString().Replace("\n", "").Replace("\r", "");
        }

        private string StatsPanel()
        {
            StringBuilder html = new();
            string ariaLabel =
                "BASIC STATISTICS SECTION: "
                + $"GENERATED IN: {Generator.stats.genTimespan} ;"
                + $"FOLDERS: {Generator.stats.folders} ;"
                + $"SKIPPED FOLDERS: {Generator.stats.skippedFolders} ;"
                + $"FILES: {Generator.Extensions.Values.Sum()} ;"
                + $"SKIPPED FILES: {Generator.stats.skippedFiles} ;"
                + $"TOTAL SIZE: {Utils.FileSystem.ComputeSize(Generator.stats.totalSizeBytes)} ;";
            html.Append(
                $"<div class=\"p\" tabindex=\"0\" aria-label=\"{ariaLabel}\"><h4>BASIC STATISTICS</h4><ul>"
            );
            html.Append($"<li><b>GENERATED IN:</b> {Generator.stats.genTimespan}</li>");
            html.Append($"<li><b>FOLDERS:</b> {Generator.stats.folders}</li>");
            html.Append($"<li><b>SKIPPED FOLDERS:</b> {Generator.stats.skippedFolders}</li>");
            html.Append($"<li><b>FILES:</b> {Generator.Extensions.Values.Sum()}</li>");
            html.Append($"<li><b>SKIPPED FILES:</b> {Generator.stats.skippedFiles}</li>");

            html.Append(
                $"<li><b>TOTAL SIZE:</b> {Utils.FileSystem.ComputeSize(Generator.stats.totalSizeBytes)}</li>"
            );
            html.Append("</ul></div>");
            html.Append(AdvnacedStatsPanel(Generator.Extensions));
            html.Append(HtmlStruct.RemainingBody());
            return html.ToString().Replace("\n", "").Replace("\r", "");
        }

        private static string AdvnacedStatsPanel(Dictionary<string, int> extensions)
        {
            if (extensions == null || !extensions.Any())
                return "";

            string[] barChartColors = new[]
            {
                "var(--ico-application)",
                "var(--ico-code)",
                "var(--ico-audio)",
                "var(--ico-comic_book)",
                "var(--ico-book)",
                "var(--guideLines)",
            };
            var extDescending = extensions.OrderByDescending(x => x.Value);
            var top5 = extDescending.Take(5).ToDictionary();
            var top5Array = top5.ToArray();

            int total = extensions.Values.Sum(),
                others = total - top5.Values.Sum(),
                actualTopCount = top5Array.Length;

            bool hasOthers = others > 0;
            int totalElementsToRender = actualTopCount + (hasOthers ? 1 : 0);

            var chartDataList = new List<(string ext, double percentage)>();

            string ariaLabel = "FILE TYPES DISTRIBUTION CHART:";
            for (int i = 0; i < actualTopCount; i++)
            {
                double pct = (double)top5Array[i].Value / total * 100;
                chartDataList.Add(
                    (string.IsNullOrEmpty(top5Array[i].Key) ? "???" : top5Array[i].Key, pct)
                );
                ariaLabel +=
                    $" {pct:0}% {(string.IsNullOrEmpty(top5Array[i].Key) ? "???" : top5Array[i].Key).Replace(".", "")} ;";
            }
            if (hasOthers)
            {
                double othersPct = (double)others / total * 100;
                chartDataList.Add(("Others", othersPct));
                ariaLabel += $" {othersPct:0}% Other file extensions ;";
            }

            StringBuilder html = new();
            // panel
            html.Append(
                $"<div class=\"p p-ext\" tabindex=\"0\" aria-label=\"{ariaLabel}\"><h4>FILE TYPES DISTRIBUTION CHART</h4>"
            );
            // bar chart
            html.Append("<div class=\"sbc-container\"><div class=\"sbc-bar\">");
            // bar chart data
            for (int i = 0; i < chartDataList.Count; i++)
            {
                string color =
                    (i == chartDataList.Count - 1 && hasOthers)
                        ? barChartColors[5]
                        : barChartColors[i % 5];
                html.Append(
                    $"<div style=\"width: {chartDataList[i].percentage:0}%; background-color: {color};\"></div>"
                );
            }
            // bar chart legend
            html.Append("</div><div class=\"sbc-l-c\">");
            for (int i = 0; i < chartDataList.Count; i++)
            {
                string color =
                    (i == chartDataList.Count - 1 && hasOthers)
                        ? barChartColors[5]
                        : barChartColors[i % 5];
                string extLabel =
                    chartDataList[i].ext == "Others"
                        ? "Others"
                        : $"{chartDataList[i].ext.ToUpper()}";
                html.Append(
                    $"<span class=\"sbc-l-el\" style=\"--sbcLegendDot: {color};\">{chartDataList[i].percentage:0}% {extLabel}</span>"
                );
            }
            html.Append("</div></div></div>");

            // all extensions list
            html.Append(
                $"<div class=\"p\" tabindex=\"0\" aria-label=\"EXTENSIONS LIST SECTION\"><h4>EXTENSIONS LIST ({extensions.Count})</h4>"
            );
            html.Append(
                $"<div class=\"other-exts\" aria-label=\"EXTENSIONS COMPLETE LIST ({extensions.Count} unique extensions)\">"
            );
            int c = 1;
            foreach (var extPair in extDescending)
                html.Append(
                    $"<span>{c++}) {((double)extPair.Value / total * 100).ToString("0.##")}% <b>{(string.IsNullOrEmpty(extPair.Key) ? "???" : extPair.Key)} ({extPair.Value})</b></span>"
                );

            html.Append("</div></div>");
            return html.ToString();
        }

        private string Footer() =>
            "];</script>"
            + HtmlStruct.Scripts(Generator.Extensions.Keys.ToArray(), Generator.metadata.dirName)
            + "</body></html>";
    }
}
