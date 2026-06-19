namespace Core.Writers
{
    public sealed class MarkdownWriter : BaseWriter
    {
        readonly GenFlags _flags = new GenFlags();

        public MarkdownWriter(GenFlags flags)
            : base(flags)
        {
            _flags = flags;
        }

        protected override async Task WriteNodeAsync(StreamWriter writer, Node node)
        {
            string? prefix = null,
                symbol = null;
            if (!_flags.noFormatting)
            {
                if (node.Level > 0)
                {
                    for (int c = 0; c < node.Level - 1; c++)
                        prefix += "  ";

                    prefix += "-";
                }

                if (!_flags.noIcons)
                {
                    switch (node.Type)
                    {
                        case NodeType.Folder:
                            symbol = "📁";
                            break;
                        case NodeType.File:
                            symbol = "📄";
                            break;
                        default:
                            symbol = null;
                            break;
                    }
                }
            }
            await writer.WriteAsync($"{prefix} {symbol} {node.Name}\r\n");
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
            "\n" + new string('-', 3) + $"\nGenerated using {AppInfo.AppName} ()";

        private string Header(Metadata data) => "# " + data.dirName + " folder structure report\n";

        private string MetadataPanel(Metadata data)
        {
            string outString = string.Empty;
            outString += "\n### REPORT METADATA";
            outString += "\n- **DIRECTORY NAME:** " + data.dirName;
            outString += "\n- **DIRECTORY PATH:** " + data.dirPath;
            outString += "\n- **PRIVILIGES:** " + data.priviliges;
            outString += "\n- **GENERATED AT:** " + data.genDateTime;
            outString += "\n---\n";
            return outString;
        }

        private string StatsPanel(Statistics data)
        {
            string outString = string.Empty;
            outString += "### STATISTICS\n";
            outString += "\n- **GENERATED IN:** " + data.genTimespan;
            outString += "\n- **FOLDERS:** " + data.folders;
            outString += "\n- **FILES:** " + data.files;
            outString += "\n- **TOTAL SIZE:** " + Utils.FileSystem.ComputeSize(data.totalSizeBytes);
            outString += "\n- **SKIPPED FOLDERS:** " + data.skippedFolders + "\n";
            outString += "\n---\n";
            return outString;
        }
    }
}
