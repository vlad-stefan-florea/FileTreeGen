using System.Diagnostics;
using System.Text;

namespace Core.Writers
{
    public class TxtWriter
    {
        GenFlags _flags = new();

        public TxtWriter(GenFlags genFlags)
        {
            _flags = genFlags;
        }

        public async Task WriteAsync()
        {
            string finalPath = _flags.outPath;
            string tempPath = Path.GetTempFileName();

            var gen = new TreeGenerator(_flags);
            var nodes = gen.GenerateTree();

            //writing the tree (temp file)
            Stopwatch sw = new();
            sw.Start();
            using (
                var fileStream = new FileStream(
                    tempPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: (int)_flags.bufferSize,
                    useAsync: true
                )
            )
            {
                using (
                    var tempWriter = new StreamWriter(
                        fileStream,
                        Encoding.UTF8,
                        (int)_flags.bufferSize,
                        leaveOpen: false
                    )
                )
                {
                    foreach (var node in nodes)
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

                            symbol = _flags.noIcons
                                ? null
                                : (node.Type == NodeType.Folder ? "[DIR] " : null);
                        }
                        string line = $"{prefix}{symbol} {node.Name}\r\n";
                        await tempWriter.WriteAsync(line);
                    }
                    await tempWriter.FlushAsync();
                }
            }
            sw.Stop();
            gen.metadata.genTimespan = sw.Elapsed.ToString(@"hh\:mm\:ss\.fff");

            //writing the metadata (final file)
            using (
                var finalStream = new FileStream(
                    finalPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    (int)_flags.bufferSize,
                    useAsync: true
                )
            )
            {
                // write metadata
                using (
                    var finalWriter = new StreamWriter(
                        finalStream,
                        Encoding.UTF8,
                        (int)_flags.bufferSize,
                        leaveOpen: false
                    )
                )
                {
                    string metadataPanel = genMetadataPanel(gen.metadata);
                    await finalWriter.WriteAsync(metadataPanel);
                    await finalWriter.FlushAsync();
                }

                // copy tree from temp file
                if (File.Exists(tempPath))
                {
                    using (
                        var fsAppend = new FileStream(
                            finalPath,
                            FileMode.Append,
                            FileAccess.Write,
                            FileShare.None,
                            (int)_flags.bufferSize,
                            useAsync: true
                        )
                    )
                    using (
                        var fsTemp = new FileStream(
                            tempPath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.None,
                            (int)_flags.bufferSize,
                            useAsync: true
                        )
                    )
                    {
                        await fsTemp.CopyToAsync(fsAppend);
                    }

                    File.Delete(tempPath);
                }
            }
        }

        private string genMetadataPanel(Metadata data)
        {
            string outString = string.Empty;
            outString += new string('-', 30);
            outString += "\nREPORT METADATA PANEL\n";
            outString += new string('-', 20);
            outString += "\nDIRECTORY NAME: " + data.dirName;
            outString += "\nDIRECTORY PATH: " + data.dirPath;
            outString += "\nPRIVILIGES: " + data.priviliges;
            outString += "\nGENERATED AT: " + data.genDateTime;
            outString += "\nGENERATED IN: " + data.genTimespan;
            outString += "\nFOLDERS: " + data.folders;
            outString += "\nFILES: " + data.files;
            outString += "\nTOTAL SIZE: " + Utils.FileSystem.ComputeSize(data.totalSizeBytes);
            outString += "\nSKIPPED FOLDERS: " + data.skippedFolders + "\n";
            outString += new string('-', 30) + "\n";
            return outString;
        }
    }
}
