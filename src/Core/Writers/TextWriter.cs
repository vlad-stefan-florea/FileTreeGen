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
            string? prefix = null,
                symbol = null;
            if (!_flags.noFormatting)
            {
                if (node.Level > 0)
                {
                    for (int c = 0; c < node.Level - 1; c++)
                        prefix += "│ ";

                    prefix += "├─";
                }

                symbol = _flags.noIcons ? null : (node.Type == NodeType.Folder ? "[DIR]" : null);
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
            "\n" + new string('-', 50) + $"\nGenerated using {AppInfo.AppName} ()";

        private string Header(Metadata data) =>
            data.dirName + " folder structure report\n" + new string('-', 50);

        private string MetadataPanel(Metadata data)
        {
            string outString = string.Empty;
            outString += "\nREPORT METADATA\n";
            outString += new string('-', 10);
            outString += "\nDIRECTORY NAME: " + data.dirName;
            outString += "\nDIRECTORY PATH: " + data.dirPath;
            outString += "\nPRIVILIGES: " + data.priviliges;
            outString += "\nGENERATED AT: " + data.genDateTime + "\n";
            outString += new string('-', 40) + "\n";
            return outString;
        }

        private string StatsPanel(Statistics data)
        {
            string outString = string.Empty;
            outString += "STATISTICS\n";
            outString += new string('-', 10);
            outString += "\nGENERATED IN: " + data.genTimespan;
            outString += "\nFOLDERS: " + data.folders;
            outString += "\nFILES: " + data.files;
            outString += "\nTOTAL SIZE: " + Utils.FileSystem.ComputeSize(data.totalSizeBytes);
            outString += "\nSKIPPED FOLDERS: " + data.skippedFolders + "\n";
            outString += new string('-', 40) + "\n";
            return outString;
        }
    }
}
