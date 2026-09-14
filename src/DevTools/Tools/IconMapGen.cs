using System.CodeDom.Compiler;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Core.Utils;
using TUI;
using static TUI.Display;

namespace DevTools.Tools
{
    internal class IconMapGen
    {
        public record IconMappingModel(
            string Category,
            string SvgPath,
            IReadOnlyList<string> Extensions
        );

        static string htmlOutPath = Path.Combine(
            SolutionUtils.GetFTGRootPath(),
            "docs",
            "IconMap.html"
        );

        public static void Run()
        {
            var categories = Core.HtmlTemplates.Icons.FileTypes;
            var svgPaths = Core.HtmlTemplates.Icons.Paths;
            List<IconMappingModel> IconMap = new();

            WriteMsg("Generating the icon map ...", MsgType.Info);

            int total = categories.Count;
            int current = 0;
            foreach (var cat in categories)
            {
                current++;
                string category = cat.Key;
                if (svgPaths.TryGetValue(category, out var svgPath))
                {
                    List<string> exts = cat.Value.ToList();
                    IconMappingModel model = new(category, svgPath, exts);
                    IconMap.Add(model);
                }
                DrawProgressBar(current, total);
            }
            Console.WriteLine();
            WriteMsg("Icon map generated", MsgType.Success);
            if (File.Exists(htmlOutPath))
            {
                File.Delete(htmlOutPath);
                WriteMsg("Previous HTML file deleted", MsgType.Warning);
            }
            WriteMsg("Writing the HTML output file ...", MsgType.Info);
            try
            {
                GenerateHTML(IconMap);
            }
            catch (Exception)
            {
                throw;
            }
            WriteMsg("HTML icon map generated", MsgType.Success);
            bool open = InputHandler.AskYN("Open the generated file?", true);
            if (open)
                FileSystem.OpenPath(htmlOutPath);
            return;

            // HELPERS
            void GenerateHTML(List<IconMappingModel> map)
            {
                string styles = """
                      :root {
                        --bkg: #f7f7f7;
                        --text: #000000;
                        --guideLines: #787878;
                        --hoverHeader: #dcdcdc;
                        --ico-expand: var(--text);
                        --ico-collapse: var(--text);
                        --ico-theme: var(--text);
                        --ico-folder_closed: #df9a00;
                        --ico-folder_open: #ffb300;
                        --ico-default: #757575;
                        --ico-text: #2b579a;
                        --ico-audio: #00a300;
                        --ico-video: #da532c;
                        --ico-image: #00828a;
                        --ico-code: #e81123;
                        --ico-link: #6c4675;
                        --ico-model: #68217a;
                        --ico-pdf: #b91d47;
                        --ico-database: #1e7145;
                        --ico-application: #0078d7;
                        --ico-archive: #fbc02d;
                        --ico-spreadsheet: #107c41;
                        --ico-presentation: #c43e1c;
                        --ico-book: #007b83;
                        --ico-comic_book: #6a1b9a;
                        --ico-torrent: #2e7d32;
                      }
                      body.dark-mode {
                        --bkg: #1f1f1f;
                        --text: #f9f9f9;
                        --guideLines: #a7a7a7;
                        --hoverHeader: #3b3b3b;
                        --ico-expand: var(--text);
                        --ico-collapse: var(--text);
                        --ico-theme: var(--text);
                        --ico-folder_closed: #ffca28;
                        --ico-folder_open: #ffe082;
                        --ico-default: #b0bec5;
                        --ico-text: #90caf9;
                        --ico-audio: #a5d6a7;
                        --ico-video: #ffab91;
                        --ico-image: #80deea;
                        --ico-code: #ef9a9a;
                        --ico-link: #d1c4e9;
                        --ico-model: #e1bee7;
                        --ico-pdf: #ff8a80;
                        --ico-database: #a5d6a7;
                        --ico-application: #29b6f6;
                        --ico-archive: #fff59d;
                        --ico-spreadsheet: #a5d6a7;
                        --ico-presentation: #ffcc80;
                        --ico-book: #80deea;
                        --ico-comic_book: #e1bee7;
                        --ico-torrent: #c5e1a5;
                      }
                      body {
                        font-family: "Segoe UI", monospace;
                        background-color: var(--bkg);
                        color: var(--text);
                      }
                      table {
                        border-collapse: collapse;
                        width: 100%;
                      }
                      td,
                      th {
                        text-align: left;
                        padding: 8px;
                      }
                      th {
                        color: var(--bkg);
                        background-color: var(--text);
                        position: sticky;
                        top: 0;
                        z-index: 10;
                      }
                      tr:nth-child(even) {
                        background-color: var(--hoverHeader);
                      }
                      svg {
                        width: 1.5rem;
                        height: 1.5rem;
                      }
                      .badge {
                        background-color: var(--bkg);
                        border: 1px solid var(--guideLines);
                        border-radius: 5px;
                        padding: 2px 4px;
                        margin-right: 5px;
                      }
                    """;
                StringBuilder html = new();
                // header
                html.Append(
                    $"""
                    <!doctype html>
                        <html lang="en">
                        <meta charset="UTF-8">
                        <meta name="viewport" content="width=device-width, initial-scale=1.0">
                        <head>
                            <title>FileTreeGen HTML Icon Map</title>
                            <style>
                                {styles}
                            </style>
                        </head>
                        <body>
                        <h2>FileTreeGen HTML Icon Map</h2>
                    """
                );
                // ICONS

                #region GENERAL_ICONS
                html.Append(
                    """
                    <h3>GENERAL ICONS</h3>
                    <table>
                        <tr>
                            <th>SVG Icon</th>
                            <th>Use</th>
                        </tr>
                    """
                );
                List<string> generalIcons = Core.HtmlTemplates.Icons.neededIconTypes.ToList();
                foreach (var entry in generalIcons)
                    html.Append(
                        $"""
                        <tr>
                            <td><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 640" fill="var(--ico-{entry}, var(--text))"><path d="{svgPaths[
                            entry
                        ]}"/></svg></td>
                            <td>{entry.Replace('_', ' ').ToUpper()}</td>
                        </tr>
                        """
                    );
                html.Append("</table>");
                #endregion
                #region FILE_ICONS
                html.Append(
                    """
                    <h3>FILE ICONS</h3>
                    <table>
                        <tr>
                            <th>SVG Icon</th>
                            <th>File Category</th>
                            <th>Extensions</th>
                        </tr>
                    """
                );
                foreach (var entry in IconMap)
                {
                    html.Append(
                        $"""
                        <tr>
                            <td><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 640" fill="var(--ico-{entry.Category}, var(--text))"><path d="{entry.SvgPath}"/></svg></td>
                            <td>{entry.Category.Replace('_', ' ').ToUpper()}</td>
                            <td>
                        """
                    );
                    foreach (var ext in entry.Extensions)
                        html.Append($"<span class=\"badge\">{ext}</span>");
                    html.Append("</td></tr>");
                }
                html.Append("</table>");
                #endregion

                // scripts
                html.Append(
                    """
                    <script>
                        function applyTheme() {
                            const prefersDark = window.matchMedia("(prefers-color-scheme: dark)",).matches;
                            if (prefersDark) {
                              document.body.classList.add("dark-mode");
                            } else {
                              document.body.classList.remove("dark-mode");
                            }
                        }
                        applyTheme();
                    </script>
                    """
                );
                html.Append("</body></html>");

                File.WriteAllText(htmlOutPath, html);
            }
        }

        private static void DrawProgressBar(int current, int total)
        {
            int barSize = 20;
            double percentage = (double)current / total * 100;
            int filled = (int)Math.Round((percentage / 100) * barSize);
            string bar = new string('▓', filled) + new string('░', barSize - filled);
            Console.Write($"\r|{bar}| {percentage:F1}% ({current}/{total})");
        }
    }
}
