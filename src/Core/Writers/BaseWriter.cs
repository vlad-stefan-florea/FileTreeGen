using System.Diagnostics;
using System.Text;

namespace Core.Writers
{
    public abstract class BaseWriter
    {
        GenFlags _flags = new();

        public BaseWriter(GenFlags genFlags)
        {
            _flags = genFlags;
        }

        protected abstract string FormatNode(Node node);
        protected abstract string GenerateMetadataPanel(Metadata data);
        protected abstract string GenerateStatsPanel(Statistics data);

        public async Task WriteAsync()
        {
            string finalPath = _flags.outPath;
            string tempPath = _flags.noStatistics ? finalPath : Path.GetTempFileName();
            // ^ if no statistics should be generated:
            // - change the temp path into the final one here (not in the file stream):
            // - write directly into the final file
            // - completely skip the second read-copy writing phase

            var gen = new TreeGenerator(_flags);
            var nodes = gen.GenerateTree();

            //writing the tree (temp file)
            Stopwatch sw = new();
            if (!_flags.noStatistics)
                sw.Start(); // it will start/stop only if 'noStatistics' is set to false (when generating them)

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
                    if (_flags.noStatistics)
                        await tempWriter.WriteAsync(GenerateMetadataPanel(gen.metadata));
                    // write basic info only when no stats are generated
                    // -> else they should be added BEFORE the stats in the second phase

                    foreach (var node in nodes)
                    {
                        await tempWriter.WriteAsync(FormatNode(node));
                    }
                    await tempWriter.FlushAsync();
                }
            }

            // write the statistics (OPTIONAL)
            if (!_flags.noStatistics)
            {
                sw.Stop();
                gen.stats.genTimespan = sw.Elapsed.ToString(@"hh\:mm\:ss\.fff");
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
                    // write metadata & stats
                    using (
                        var finalWriter = new StreamWriter(
                            finalStream,
                            Encoding.UTF8,
                            (int)_flags.bufferSize,
                            leaveOpen: false
                        )
                    )
                    {
                        string metadataPanel = GenerateMetadataPanel(gen.metadata);
                        string statsPanel = GenerateStatsPanel(gen.stats);
                        await finalWriter.WriteAsync(metadataPanel + statsPanel);
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
        }
    }
}
