namespace Core.Writers
{
    public sealed class TextWriter : BaseWriter
    {
        GenFlags _flags = new GenFlags();

        public TextWriter(GenFlags flags)
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
                        prefix += "│ ";

                    prefix += "├─";
                }

                symbol = _flags.noIcons ? null : (node.Type == NodeType.Folder ? "[DIR]" : null);
            }
            return $"{prefix}{symbol} {node.Name}\r\n";
        }

        protected override string GenerateMetadataPanel(Metadata data)
        {
            string outString = string.Empty;
            outString += data.dirName + " FileTreeGen report\n";
            outString += new string('-', 50);
            outString += "\nREPORT METADATA\n";
            outString += new string('-', 10);
            outString += "\nDIRECTORY NAME: " + data.dirName;
            outString += "\nDIRECTORY PATH: " + data.dirPath;
            outString += "\nPRIVILIGES: " + data.priviliges;
            outString += "\nGENERATED AT: " + data.genDateTime + "\n";
            outString += new string('-', 40) + "\n";
            return outString;
        }

        protected override string GenerateStatsPanel(Statistics data)
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
