using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;

namespace FolderStructurer
{
    public class Node
    {
        public string Name { get; set; } = "";
        public bool IsFile { get; set; }
        public string Extension { get; set; } = "";
        public string PathIfFile { get; set; } = "";
        public List<Node> Children { get; set; } = new();
    }

    public class FileSysUtils
    {
        public static readonly Dictionary<string, int> FileCounter;
        public static readonly Dictionary<string, (string Class, string Color)> IconMap = new();
        public static int FolderCount = 0;
        public static long TotalSize = 0;

        private static bool isFolder(string? lPath) =>
            string.IsNullOrEmpty(lPath) ? false : Directory.Exists(cleanPath(lPath));

        private static string cleanPath(string path) => path.Trim().Replace("\"", "");

        public static string getDir(string? argPath = null)
        {
            string? currentPath = argPath;

            if (!string.IsNullOrEmpty(currentPath) && !isFolder(currentPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️ Invalid folder provided via arguments.");
                Console.ForegroundColor = ConsoleColor.White;
                currentPath = null;
            }

            while (!isFolder(currentPath))
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("➡️ Drag & Drop the folder here: ");
                Console.ForegroundColor = ConsoleColor.White;
                currentPath = Console.ReadLine();
            }

            return cleanPath(currentPath!);
        }

        public static void LogException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ AN ERROR OCCURED WHILE TRYING TO MAP THE FOLDER STRUCTURE:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠️ Details:\n" + ex);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static Node? MapFileTree(string lPath, List<string> filter)
        {
            DirectoryInfo dir = new(lPath);
            Node folderNode = new() { Name = dir.Name, IsFile = false };
            bool allowAll = filter == null || !filter.Any();
            try
            {
                try
                {
                    foreach (var file in dir.GetFiles())
                    {
                        if (allowAll || filter.Contains(file.Extension.ToLower()))
                        {
                            folderNode.Children.Add(
                                new()
                                {
                                    Name = file.Name,
                                    IsFile = true,
                                    Extension = file.Extension.ToLower(),
                                    PathIfFile = file.FullName,
                                }
                            );
                            if (FileCounter.ContainsKey(file.Extension))
                                FileCounter[file.Extension]++;
                            else
                                FileCounter[file.Extension] = 1;
                            TotalSize += file.Length;
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ Skipped folder '{dir}' (Unauthorized Access)");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                try
                {
                    foreach (var directory in dir.GetDirectories())
                    {
                        folderNode.Children.Add(MapFileTree(directory.FullName, filter));
                        FolderCount++;
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ Skipped folder '{dir}' (Unauthorized Access)");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
            return folderNode;
        }

        enum Scales
        {
            Bytes,
            KB,
            MB,
            GB,
            TB,
        }

        public static string ComputeSize(long sizeBytes)
        {
            double size = sizeBytes;
            int scale = 0;
            while (size >= 1024 && scale < Enum.GetValues(typeof(Scales)).Length - 1)
            {
                size /= 1024;
                scale++;
            }
            return $"{size:0.##} {(Scales)scale}";
        }

        public static bool IsRunningAsAdmin()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        static FileSysUtils()
        {
            FileCounter = new();
            var mappings = new[]
            {
                (
                    new[] { ".cs", ".html", ".css", ".js", ".json", ".py", ".cpp", ".h", ".ts" },
                    "fa-file-code",
                    "#2ecc71"
                ),
                (
                    new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp" },
                    "fa-file-image",
                    "#3498db"
                ),
                (new[] { ".pdf" }, "fa-file-pdf", "#e74c3c"),
                (new[] { ".zip", ".rar", ".7z", ".tar", ".gz" }, "fa-file-zipper", "#f1c40f"),
                (new[] { ".mp3", ".wav", ".ogg" }, "fa-file-audio", "#9b59b6"),
                (new[] { ".mp4", ".mov", ".avi", ".mkv" }, "fa-file-video", "#e67e22"),
                (new[] { ".txt", ".log", ".md" }, "fa-file-lines", "#9faeaf"),
            };
            foreach (var (extensions, iconClass, color) in mappings)
            {
                foreach (var ext in extensions)
                {
                    IconMap[ext] = (iconClass, color);
                }
            }
        }
    }

    public class HtmlUtils
    {
        private static string GetIconHtml(string extension)
        {
            string ext = extension.ToLower();

            if (FileSysUtils.IconMap.TryGetValue(ext, out var config))
            {
                return $"<i class='fa-solid {config.Class}' style='color: {config.Color}'></i>";
            }

            return "<i class='fa-solid fa-file' style='color: var(--text)'></i>";
        }

        private static string? DrawFileNode(Node fileNode) =>
            "<a class=\"file-item\" href=\""
            + "file:///"
            + fileNode.PathIfFile.Replace("\\", "/")
            + "\" target=\"_blank\">"
            + GetIconHtml(fileNode.Extension)
            + fileNode.Name
            + "</a>";

        private static void DrawFolderNode(Node folderNode, StringBuilder sb)
        {
            var sortedChildren = folderNode.Children.OrderBy(c => c.IsFile).ThenBy(c => c.Name);
            sb.Append("<div class=\"folder-container\">");
            string outHtml =
                $"<div class=\"folder-header\" onclick=\"toggle(this)\" {(folderNode.Children.Any() ? "" : "style=\"cursor: default !important\"")}>"
                + $"<span class=\"toggle-icon {(folderNode.Children.Any() ? "hasContents" : "")}\">"
                + (folderNode.Children.Any() ? "[+] 📁" : "[#] 📁")
                + $"</span>&nbsp;{folderNode.Name + (folderNode.Children.Any() ? "" : " (Empty)")}</div>";
            sb.Append(outHtml);
            if (folderNode.Children.Any())
            {
                sb.Append("<div class=\"folder-content hidden\">");
                foreach (var child in sortedChildren)
                {
                    if (child.IsFile)
                        sb.Append(DrawFileNode(child));
                    else
                        DrawFolderNode(child, sb);
                }
                sb.Append("</div>");
            }
            sb.Append("</div>");
        }

        private static void DrawStyles(StringBuilder sb)
        {
            string css = """
                <style>
                  * {
                    box-sizing: border-box;
                  }

                  :root {
                    --bkg: #f7f7f7;
                    --text: #000000;
                    --guideLines: #787878;
                    --hoverHeader: #dcdcdc;
                    --fileItem: #444;
                    --accent: #3498db;
                  }

                  body.dark-mode {
                    --bkg: #1f1f1f;
                    --text: #f9f9f9;
                    --guideLines: #a7a7a7;
                    --hoverHeader: #3b3b3b;
                    --fileItem: #d3d3d3;
                  }

                  body {
                    max-width: 100%;
                    font-family: "Segoe UI", monospace;
                    background-color: var(--bkg);
                    color: var(--text);
                    transition:
                      background 0.3s,
                      color 0.3s;
                  }
                  
                  .folder-header {
                    cursor: pointer;
                    padding: 2px 5px;
                    display: flex;
                    align-items: center;
                    user-select: none;
                    position: relative;
                    transition:
                      background 0.2s,
                      font-weight 0.2s;
                  }
                  
                  .folder-header:hover {
                    background: var(--hoverHeader);
                    font-weight: bold;
                  }

                  .folder-content {
                    margin-left: 30px;
                    border-left: 2px solid var(--guideLines);
                    display: block;
                  }
                  
                  .file-item {
                    padding-left: 20px;
                    position: relative;
                    display: flex;
                    align-items: center;
                    padding-block: 2px;
                    gap: 4px;
                    color: var(--fileItem);
                    text-decoration: none;
                    width: 100%;
                    transition:
                      background 0.2s,
                      font-weight 0.2s;
                  }
                  
                  .file-item:hover {
                    background: var(--hoverHeader);
                    color: var(--text);
                    font-weight: bold;
                  }
                  
                  .file-item::before {
                    content: "";
                    position: absolute;
                    left: 0;
                    top: 50%;
                    width: 15px;
                    height: 2px;
                    background: var(--guideLines);
                  }

                  .file-item i {
                    width: 20px;
                    text-align: center;
                    flex-shrink: 0;
                  }

                  button {
                    position: relative;
                    cursor: pointer;
                    border: 1px solid var(--text);
                    padding: 5px 10px;
                    transition: all 0.3s;
                    color: var(--text);
                    background-color: var(--bkg);
                    margin-right: 5px;
                  }
                  
                  button:hover {
                    background-color: var(--hoverHeader);
                  }
                  
                  .navbar {
                    position: sticky;
                    top: 0;
                    display: flex;
                    flex-direction: row;
                    justify-content: space-between;
                    align-items: center;
                    padding: 5px;
                    user-select: none;
                    background-color: var(--bkg);
                    border: 2px solid var(--fileItem);
                    z-index: 2;
                  }
                  
                  .bar-item {
                    margin-block: 10px;
                    align-items: center;
                    position: relative;
                    display: flex;
                    flex-direction: row;
                  }

                  .bar-label {
                    margin-left: 10px;
                    line-height: 100%;
                  }

                  .bar-track {
                    width: 30%;
                    height: 16px;
                    border: 2px solid var(--guideLines);
                  }

                  .bar-fill {
                    height: 100%;
                    background-color: var(--accent);
                    overflow: hidden;
                  }

                  p {
                    margin-block: 0;
                  }

                  .others-details {
                    overflow: visible;
                    display: flex;
                    flex-direction: row;
                    flex-wrap: wrap;
                  }
                  
                  .other-ext {
                    margin: 5px;
                    border: 1px solid var(--fileItem);
                    align-items: center;
                    justify-content: center;
                    padding: 5px;
                    border-radius: 10px;
                  }
                  .hidden {
                    display: none;
                  }
                </style>
                """;
            sb.Append(css);
        }

        private static void DrawHead(string fileName, StringBuilder sb)
        {
            string first =
                $"<title>{fileName}</title>"
                + """
                    <link
                      rel="stylesheet"
                      href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css"
                    />
                    """;
            sb.Append("<head>");
            sb.Append(first);
            DrawStyles(sb);
            sb.Append("</head>");
        }

        private static void DrawNavbar(StringBuilder sb)
        {
            string navbar = """
                <div class="navbar">
                  <div class="nav-buttons">
                    <button onclick="expandAll()">
                      <i class="fa-solid fa-angles-down"></i> Expand All
                    </button>
                    <button onclick="collapseAll()">
                      <i class="fa-solid fa-angles-up"></i> Collapse All
                    </button>
                    <button onclick="toggleTheme()">
                      <i class="fa-solid fa-circle-half-stroke"></i> Theme
                    </button>
                  </div>
                </div>
                """;
            sb.Append(navbar);
        }

        private static void DrawBody(
            Node rootNode,
            string rootPath,
            string appName,
            StringBuilder sb
        )
        {
            sb.Append("<body>");
            DrawNavbar(sb);
            sb.Append("<hr/>");

            sb.Append($"<p>'{Path.GetFileName(rootPath)}' File Tree ({rootPath})</p>");
            sb.Append($"<p>Creation Date: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}</p>");
            sb.Append(
                $"<p>Privileges: {(FileSysUtils.IsRunningAsAdmin() ? "Administrator" : "Standard User")}</p>"
            );

            int totalFiles = FileSysUtils.FileCounter.Values.Sum();
            sb.Append(
                $"<p>Folders: {FileSysUtils.FolderCount} | Files: {totalFiles} | Size: {FileSysUtils.ComputeSize(FileSysUtils.TotalSize)}</p>"
            );

            sb.Append("<hr/>");
            DrawFolderNode(rootNode, sb);

            if (totalFiles > 0)
            {
                sb.Append("<hr /><u>File Types Distribution</u>");

                // bar chart helper
                string GenerateBar(string key) =>
                    $@"
                    <div class='bar-item'>
                        <div class='bar-track'>
                            <div class='bar-fill' style='width:{((double)FileSysUtils.FileCounter[key] / totalFiles * 100).ToString("F2")}%;'></div>
                        </div>
                        <div class='bar-label'>{(key == "" ? "Unknown" : key)} ({FileSysUtils.FileCounter[key]})</div>
                    </div>
                    ";

                var keys = FileSysUtils
                    .FileCounter.Keys.OrderByDescending(k => FileSysUtils.FileCounter[k])
                    .ToList();

                int limit = Math.Min(5, keys.Count);
                for (int i = 0; i < limit; i++)
                    sb.Append(GenerateBar(keys[i]));

                if (keys.Count > 5)
                {
                    int sum = 0;
                    List<string> otherKeys = new();
                    for (int i = 5; i < keys.Count; i++)
                    {
                        sum += FileSysUtils.FileCounter[keys[i]];
                        otherKeys.Add(keys[i]);
                    }
                    sb.Append(
                        $@"
                        <div class='bar-item'>
                            <div class='bar-track'>
                                <div class='bar-fill' style='width:{((double)sum / totalFiles * 100).ToString("F2")}%;'></div>
                            </div>
                            <div class='bar-label'>{"Others" + ""} ({sum})</div>
                        </div>
                        "
                    );
                    sb.Append($"Others:\n<div class='others-details'>");
                    foreach (string k in otherKeys)
                    {
                        sb.Append($"<span class='other-ext'>{k}</span>");
                    }
                    sb.Append($"</div>");
                }
            }
            sb.Append($"<hr /><footer>Generated using {appName}</footer>");
            sb.Append("</body>");
        }

        private static void DrawScritps(StringBuilder sb)
        {
            string scripts = """
                <script>
                  function toggle(element) {
                    const content = element.nextElementSibling;
                    const icon = element.querySelector(".toggle-icon");

                    if (content.classList.contains("hidden")) {
                      content.classList.remove("hidden");
                      icon.innerText = "[-] 📂";
                    } else {
                      content.classList.add("hidden");
                      icon.innerText = "[+] 📁";
                    }
                  }

                  function toggleTheme() {
                    document.body.classList.toggle("dark-mode");
                  }
                  function expandAll() {
                    document
                      .querySelectorAll(".folder-content")
                      .forEach((el) => el.classList.remove("hidden"));
                    document
                      .querySelectorAll(".toggle-icon.hasContents")
                      .forEach((el) => (el.innerText = "[-] 📂"));
                  }

                  function collapseAll() {
                    document
                      .querySelectorAll(".folder-content")
                      .forEach((el) => el.classList.add("hidden"));
                    document
                      .querySelectorAll(".toggle-icon.hasContents")
                      .forEach((el) => (el.innerText = "[+] 📁"));
                  }
                </script>
                """;
            sb.Append(scripts);
        }

        public static void DrawHtml(
            Node rootNode,
            string rootPath,
            string appName,
            string fileName,
            StringBuilder sb
        )
        {
            sb.Append("<!doctype html>");
            sb.Append("<html>");
            DrawHead(fileName, sb);
            DrawBody(rootNode, rootPath, appName, sb);
            DrawScritps(sb);
            sb.Append("</html>");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // params
                string dirPath = string.Empty,
                    appName = "File Tree Generator";

                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;
                Console.ForegroundColor = ConsoleColor.White;

                string? initialArg = args.Length > 0 ? args[0] : null;
                List<string> filter = new();

                foreach (string arg in args)
                {
                    if (arg.StartsWith("-filter:", StringComparison.OrdinalIgnoreCase))
                    {
                        var formats = arg.Substring("-filter:".Length)
                            .Split(';', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var f in formats)
                        {
                            string cleanFormat = f.Trim().ToLower();
                            if (!cleanFormat.StartsWith("."))
                                cleanFormat = "." + cleanFormat;
                            filter.Add(cleanFormat);
                        }
                    }
                }

                dirPath = FileSysUtils.getDir(initialArg);
                string fileName = Path.GetFileName(dirPath) + "_file-tree.html";

                Node? rootNode = FileSysUtils.MapFileTree(dirPath, filter);

                StringBuilder sb = new StringBuilder();

                HtmlUtils.DrawHtml(rootNode, dirPath, appName, fileName, sb);

                string finalCode = sb.ToString();
                string downloadsPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "Downloads"
                    ),
                    outFilePath = Path.Combine(downloadsPath, fileName);
                File.WriteAllText(outFilePath, finalCode.Trim(), Encoding.UTF8);

                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = outFilePath,
                        UseShellExecute = true,
                    }
                );
            }
            catch (Exception ex)
            {
                FileSysUtils.LogException(ex);
            }
        }
    }
}
