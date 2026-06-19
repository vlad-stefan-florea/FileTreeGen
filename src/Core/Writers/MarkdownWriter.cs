namespace Core.Writers
{
    public sealed class MarkdownWriter : BaseWriter
    {
        GenFlags _flags = new GenFlags();

        public MarkdownWriter(GenFlags flags)
            : base(flags)
        {
            _flags = flags;
        }

        protected override string FormatNode(Node node)
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
            return $"{prefix} {symbol} {node.Name}\r\n";
        }

        protected override string GenerateMetadataPanel(Metadata data)
        {
            string outString = string.Empty;
            outString += "# " + data.dirName + " FileTreeGen report";
            outString += "\n### REPORT METADATA";
            outString += "\n- **DIRECTORY NAME:** " + data.dirName;
            outString += "\n- **DIRECTORY PATH:** " + data.dirPath;
            outString += "\n- **PRIVILIGES:** " + data.priviliges;
            outString += "\n- **GENERATED AT:** " + data.genDateTime;
            outString += "\n---\n";
            return outString;
        }

        protected override string GenerateStatsPanel(Statistics data)
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
