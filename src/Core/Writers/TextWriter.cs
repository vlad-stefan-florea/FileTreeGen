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
            if (!_flags.noFormatting)
            {
                if (node.Level > 0)
                {
                    for (int c = 0; c < node.Level - 1; c++)
                        prefix += "│ ";
                    prefix += "├─";
                }
                if (!_flags.noIcons)
                {
                    symbol = node.Type switch
                    {
                        NodeType.Folder => "[DIR]",
                        NodeType.File => string.Empty,
                        _ => string.Empty,
                    };
                }
            }
            await writer.WriteAsync($"{prefix}{symbol} {node.Name}\r\n");
        }

        protected override async Task WriteMetadataAsync(StreamWriter writer, Metadata metadata) =>
            await writer.WriteAsync(MetadataPanel(metadata));

        protected override async Task WriteStatisticsAsync(StreamWriter writer, Statistics stats) =>
            await writer.WriteAsync(StatsPanel(stats));

        protected override async Task WriteHeaderAsync(StreamWriter writer, Metadata metadata) =>
            await writer.WriteAsync(Header(metadata));

        protected override async Task WriteFooterAsync(StreamWriter writer) =>
            await writer.WriteAsync(Footer());

        private string Footer() =>
            $"\n{new string('-', 50)}\nGenerated using {AppInfo.AppName} ({AppInfo.AppUrl})";

        private string Header(Metadata data) =>
            $"\'{data.dirName}\' folder structure report\n{new string('-', 50)}\n";

        private string MetadataPanel(Metadata m) =>
            $"""
                REPORT METADATA
                -----------------
                TARGET DIRECTORY: {m.dirName}
                TARGET PATH: {m.dirPath}
                ACESS LEVEL: {m.accessLevel}
                GENERATED AT: {m.genDateTime}
                ----------------------------------------
                """ + "\n";

        private string StatsPanel(Statistics s) =>
            $"""
                STATISTICS
                ------------
                GENERATED IN: {s.genTimespan}
                FOLDERS: {s.folders}
                FILES: {s.files}
                TOTAL SIZE: {Utils.FileSystem.ComputeSize(s.totalSizeBytes)}
                SKIPPED FOLDERS: {s.skippedFolders}
                ----------------------------------------
                """ + "\n";
    }
}
