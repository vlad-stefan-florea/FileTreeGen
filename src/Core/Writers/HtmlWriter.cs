using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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
            + "<div class=\"panelsRow\">";

        private class HtmlStruct
        {
            internal static string HtmlStart(string dirName) =>
                $"<!doctype html><html><head><title>{Utils.ReportInfo.GenerateName(dirName)}</title>";

            internal static string Styles()
            {
                StringBuilder htmlHeader = new();
                htmlHeader.Append("<style>");
                htmlHeader.Append(
                    Utils.EmbeddedReader.ReadEmbeddedResource("Core.HtmlTemplates.styles.css")
                );
                htmlHeader.Append("</style>");
                return htmlHeader.ToString();
            }

            internal static string RemainingBody() =>
                "</div><hr/>"
                + "<div id=\"treeContainer\"></div>"
                + $"<hr/><footer>Generated using <a href=\"{AppInfo.AppUrl}\" target=\"_blank\">{AppInfo.AppName}</a></footer>"
                + "<script>const nodeList=[";

            internal static string Scripts(string[] foundExtensions, string dirName)
            {
                StringBuilder html = new();

                if (!_flags.noIcons)
                    html.Append(HtmlTemplates.Icons.GenSvgCollectionHtml(foundExtensions));
                html.Append(
                    $"<script>const noIcons={(_flags.noIcons ? "true" : "false")};</script>"
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
                return html.ToString().Replace("___dirName___", dirName);
            }
        }

        private string MetadataPanel()
        {
            StringBuilder html = new();
            html.Append("<div class=\"panel\"><h4>REPORT METADATA</h4><ul>");
            html.Append($"<li><b>TARGET DIRECTORY:</b> {Generator.metadata.dirName}</li>");
            html.Append($"<li><b>TARGET PATH:</b> {Generator.metadata.dirPath}</li>");
            html.Append($"<li><b>ACCESS LEVEL:</b> {Generator.metadata.accessLevel}</li>");
            html.Append($"<li><b>GENERATED AT:</b> {Generator.metadata.genDateTime}</li>");
            html.Append("</ul></div>");
            if (_flags.noStatistics)
                html.Append(HtmlStruct.RemainingBody());
            return html.ToString();
        }

        private string StatsPanel()
        {
            StringBuilder html = new();
            html.Append("<div class=\"panel\"><h4>STATISTICS</h4><ul>");
            html.Append($"<li><b>GENERATED IN:</b> {Generator.stats.genTimespan}</li>");
            html.Append($"<li><b>FOLDERS:</b> {Generator.stats.folders}</li>");
            html.Append($"<li><b>SKIPPED FOLDERS:</b> {Generator.stats.skippedFolders}</li>");
            html.Append($"<li><b>FILES:</b> {Generator.Extensions.Values.Sum()}</li>");
            html.Append(
                $"<li><b>TOTAL SIZE:</b> {Utils.FileSystem.ComputeSize(Generator.stats.totalSizeBytes)}</li>"
            );
            html.Append("</ul></div>");
            html.Append(HtmlStruct.RemainingBody());
            return html.ToString();
        }

        private string Footer() =>
            "];</script>"
            + HtmlStruct.Scripts(Generator.Extensions.Keys.ToArray(), Generator.metadata.dirName)
            + "</body></html>";
    }
}
