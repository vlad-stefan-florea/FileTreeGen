namespace Core.Writers
{
    public sealed class TextWriter : BaseWriter
    {
        readonly GenFlags _flags = new GenFlags();

        public TextWriter(GenFlags flags)
            : base(flags)
        {
            _flags = flags;
        }

        protected override async Task WriteNodeAsync(StreamWriter writer, Node node)
        {
            string? prefix = string.Empty,
                symbol = string.Empty;
            if (_flags.formatReport)
            {
                if (node.Level > 0)
                {
                    for (int c = 0; c < node.Level - 1; c++)
                        prefix += "│ ";
                    prefix += "├─";
                }
                if (_flags.includeIcons)
                    symbol = node.IsFile ? string.Empty : "[DIR]";
                if (node.IsEmptyDir)
                    node.Name += " (Empty)";
                else if (node.IsSkipped)
                    node.Name += " (Skipped)";
                else if (node.IsUnscanned)
                    node.Name += " (Not Scanned)";
            }
            await writer.WriteAsync($"{prefix}{symbol} {node.Name}".Trim() + "\r\n");
        }

        protected override async Task WriteMetadataAsync(StreamWriter writer) =>
            await writer.WriteAsync(MetadataPanel());

        protected override async Task WriteStatisticsAsync(StreamWriter writer) =>
            await writer.WriteAsync(StatsPanel());

        protected override async Task WriteHeaderAsync(StreamWriter writer) =>
            await writer.WriteAsync(Header());

        protected override async Task WriteFooterAsync(StreamWriter writer) =>
            await writer.WriteAsync(Footer());

        private string Footer() =>
            $"\n{new string('-', 50)}\nGenerated using {AppInfo.AppName} ({AppInfo.AppUrl})";

        private string Header() =>
            $"\'{Generator.metadata.dirName}\' folder structure report\n{new string('-', 50)}\n";

        private string MetadataPanel() =>
            $"""
                REPORT METADATA
                -----------------
                TARGET DIRECTORY: {Generator.metadata.dirName}
                TARGET PATH: {Generator.metadata.dirPath}
                ACESS LEVEL: {Generator.metadata.accessLevel}
                GENERATED AT: {Generator.metadata.genDateTime}
                ----------------------------------------
                """ + "\n";

        private string StatsPanel() =>
            $"""
                STATISTICS
                ------------
                GENERATED IN: {Generator.stats.genTimespan}
                FOLDERS: {Generator.stats.folders}
                SKIPPED FOLDERS: {Generator.stats.skippedFolders}
                FILES: {Generator.Extensions.Values.Sum()}
                SKIPPED FILES: {Generator.stats.skippedFiles}
                UNIQUE EXTENSIONS: {Generator.Extensions.Count}
                TOTAL SIZE: {Utils.FileSystem.ComputeSize(Generator.stats.totalSizeBytes)}
                ----------------------------------------
                """ + "\n";
    }
}
