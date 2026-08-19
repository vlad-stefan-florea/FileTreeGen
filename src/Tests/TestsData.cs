namespace Tests
{
    internal class TestsData
    {
        public static Dictionary<string, (string? Args, int ExpectedCode)> Basic = new()
        // test name, test arguments (as a single string), expected exit code
        {
            #region GENERAL
            ["Default"] = (null, 0),
            ["HTML"] = ("--type html", 0),
            ["Markdown"] = ("--type markdown", 0),
            ["Text"] = ("--type text", 0),
            ["Buffer (Small)"] = ("--buffer small", 0),
            ["Buffer (Medium)"] = ("--buffer medium", 0),
            ["Buffer (Big)"] = ("--buffer big", 0),
            ["Buffer (Large)"] = ("--buffer large", 0),
            ["Buffer (Extra Large)"] = ("--buffer extralarge", 0),
            ["Buffer (High Performance)"] = ("--buffer highperformance", 0),
            ["Report Name (Name Only)"] = ("--report-name nameonly", 0),
            ["Report Name (Name Date)"] = ("--report-name namedate", 0),
            ["Report Name (Name Date Time)"] = ("--report-name namedatetime", 0),
            ["Node Label (Name)"] = ("--node-label name", 0),
            ["Node Label (Full Path)"] = ("--node-label fullpath", 0),
            ["Node Label (Relative Path)"] = ("--node-label relativepath", 0),
            ["Whitelist"] = ("--whitelist \"dll\"", 0),
            ["Blacklist"] = ("--blacklist \"dll\"", 0),
            ["Dirs Only"] = ("--dirs-only", 0),
            ["Files Only"] = ("--files-only", 0),
            ["Max Depth (OK)"] = ("--max-depth 10", 0),
            ["Ignore Empty Dirs"] = ("--ignore-empty-dirs", 0),
            ["Ignore Symlinks"] = ("--ignore-symlinks", 0),
            ["No Stats"] = ("--no-stats", 0),
            ["No Formatting"] = ("--no-formatting", 0),
            ["No Icons"] = ("--no-icons", 0),
            ["Auto Open"] = ("--auto-open", 0),
            ["Tree Only"] = ("--tree-only", 0),
            #endregion
            #region EDGE CASES
            ["Max Depth (<= 0)"] = ("--max-depth -1", 200),
            ["Max Depth (> max int)"] = ("--max-depth " + int.MaxValue + 1, 200),
            #endregion
        };
    }
}
