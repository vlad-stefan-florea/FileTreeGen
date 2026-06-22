using System.Net;
using System.Text;
using System.Text.Json;

namespace Core.Writers
{
    public sealed class HtmlWriter : BaseWriter
    {
        private static bool isFirstNode = true;
        private static string? _headerCache;
        private static string? _footerCache;
        private static GenFlags _flags;

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
            await writer.WriteAsync(MetadataPanel()); // Înăuntru poți folosi și stats dacă ai nevoie!

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
            + HtmlStruct.Navbar()
            + $"<h1>'{Generator.metadata.dirName}' folder structure report</h1>";

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

            internal static string Navbar() =>
                """
                    <div class="navbar">
                      <div class="nav-buttons">
                        <button onclick="expandAll()" aria-label="Expand all nodes">
                          <i class="fa-solid fa-angles-down"></i> Expand All
                        </button>
                        <button onclick="collapseAll()" aria-label="Collapse all nodes">
                          <i class="fa-solid fa-angles-up"></i> Collapse All
                        </button>
                        <button onclick="toggleTheme()" aria-label="Change Theme">
                          <i class="fa-solid fa-circle-half-stroke"></i> Theme
                        </button>
                      </div>
                    </div>
                    """;

            internal static string RemainingBody() =>
                "<div id=\"treeContainer\"></div>"
                + $"<hr/><footer>Generated using <a href=\"{AppInfo.AppUrl}\" target=\"_blank\">{AppInfo.AppName}</a></footer>"
                + "<script>const nodeList=[";

            internal static string Scripts(string[] foundExtensions)
            {
                StringBuilder html = new();
                html.Append(HtmlTemplates.Icons.GenSvgCollectionHtml(foundExtensions));
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
                return html.ToString();
            }
        }

        private string MetadataPanel()
        {
            StringBuilder html = new();
            html.Append("METADATA");
            if (_flags.noStatistics)
                html.Append(HtmlStruct.RemainingBody());
            return html.ToString();
        }

        private string StatsPanel()
        {
            StringBuilder html = new();
            html.Append("STATISTICS<hr/>");
            html.Append(HtmlStruct.RemainingBody());
            return html.ToString();
        }

        private string Footer() =>
            "];</script>"
            + HtmlStruct.Scripts(Generator.metadata.Extensions.Keys.ToArray())
            + "</body></html>";
    }
}
