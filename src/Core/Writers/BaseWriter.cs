using System.Diagnostics;
using System.Text;

namespace Core.Writers
{
    public abstract class BaseWriter
    {
        GenFlags _flags = new();
        protected TreeGenerator Generator { get; private set; }

        public BaseWriter(GenFlags genFlags)
        {
            _flags = genFlags;
            Generator = new TreeGenerator(_flags);
        }

        protected abstract Task WriteHeaderAsync(StreamWriter writer);
        protected abstract Task WriteMetadataAsync(StreamWriter writer);
        protected abstract Task WriteNodeAsync(StreamWriter writer, Node node);
        protected abstract Task WriteStatisticsAsync(StreamWriter writer);
        protected abstract Task WriteFooterAsync(StreamWriter writer);

        public async Task WriteAsync()
        {
            string finalPath = _flags.outPath;
            string tempPath =
                (!_flags.includeStatistics || _flags.treeOnly) ? finalPath : Path.GetTempFileName();
            // ^ if no statistics should be generated:
            // - change the temp path into the final one here (not in the file stream):
            // - write directly into the final file
            // - completely skip the second read-copy writing phase
            var nodes = Generator.GenerateTree();

            //writing the tree (temp file)
            Stopwatch sw = new();
            if (_flags.includeStatistics)
                sw.Start(); // it will start/stop only if 'noStatistics' is set to false (when generating them)
            try
            {
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
                            new UTF8Encoding(false),
                            (int)_flags.bufferSize,
                            leaveOpen: false
                        )
                    )
                    {
                        if (!_flags.includeStatistics && !_flags.treeOnly)
                        {
                            await WriteHeaderAsync(tempWriter);
                            await WriteMetadataAsync(tempWriter);
                        }
                        // write basic info only when no stats are generated
                        // -> if stats are generated they should be added BEFORE the stats
                        // the order would be:
                        // header -> metadata -> .................... -> stats -> | final
                        //                    -> tree + footer (temp) -> ..... -> | file

                        foreach (var node in nodes)
                        {
                            await WriteNodeAsync(tempWriter, node);
                        }
                        if (!_flags.treeOnly)
                            await WriteFooterAsync(tempWriter);
                        await tempWriter.FlushAsync();
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CoreException(ExitCodes.CannotWriteOutput, ex.Message);
            }
            catch (IOException ex)
            {
                throw new CoreException(ExitCodes.CannotWriteOutput, ex.Message);
            }

            // write the statistics (OPTIONAL)
            if (_flags.includeStatistics && !_flags.treeOnly)
            {
                try
                {
                    sw.Stop();
                    Generator.stats.genTimespan = sw.Elapsed.ToString(@"hh\:mm\:ss\.fff");
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
                                new UTF8Encoding(false),
                                (int)_flags.bufferSize,
                                leaveOpen: false
                            )
                        )
                        {
                            await WriteHeaderAsync(finalWriter);
                            await WriteMetadataAsync(finalWriter);
                            if (_flags.includeStatistics)
                                await WriteStatisticsAsync(finalWriter);
                            await finalWriter.FlushAsync();
                        }

                        // copy tree (& footer) from temp file
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
                catch (UnauthorizedAccessException ex)
                {
                    throw new CoreException(ExitCodes.CannotWriteOutput, ex.Message);
                }
                catch (IOException ex)
                {
                    throw new CoreException(ExitCodes.CannotWriteOutput, ex.Message);
                }
            }
        }
    }
}
