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

        protected abstract Task WriteHeaderAsync(StreamWriter writer, Metadata metadata);
        protected abstract Task WriteMetadataAsync(StreamWriter writer, Metadata metadata);
        protected abstract Task WriteNodeAsync(StreamWriter writer, Node node);
        protected abstract Task WriteStatisticsAsync(StreamWriter writer, Statistics stats);
        protected abstract Task WriteFooterAsync(StreamWriter writer);

        public async Task WriteAsync()
        {
            var generator = new TreeGenerator(_flags);

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
                            Encoding.UTF8,
                            (int)_flags.bufferSize,
                            leaveOpen: false
                        )
                    )
                    {
                        if (_flags.noStatistics)
                        {
                            await WriteHeaderAsync(tempWriter, gen.metadata);
                            await WriteMetadataAsync(tempWriter, gen.metadata);
                        }
                        // write basic info only when no stats are generated
                        // -> else they should be added BEFORE the stats in the second phase

                        foreach (var node in nodes)
                        {
                            await WriteNodeAsync(tempWriter, node);
                        }
                        await tempWriter.FlushAsync();
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new CoreException(ErrorCode.Codes.CannotWriteOutput, ex.Message);
            }
            catch (IOException ex)
            {
                throw new CoreException(ErrorCode.Codes.CannotWriteOutput, ex.Message);
            }

            // write the statistics (OPTIONAL)
            if (!_flags.noStatistics)
            {
                try
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
                            await WriteHeaderAsync(finalWriter, gen.metadata);
                            await WriteMetadataAsync(finalWriter, gen.metadata);
                            await WriteStatisticsAsync(finalWriter, gen.stats);
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

                        // write footer
                        using (
                            var finalWriter = new StreamWriter(
                                finalStream,
                                Encoding.UTF8,
                                (int)_flags.bufferSize,
                                leaveOpen: false
                            )
                        )
                        {
                            await WriteFooterAsync(finalWriter);
                            await finalWriter.FlushAsync();
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new CoreException(ErrorCode.Codes.CannotWriteOutput, ex.Message);
                }
                catch (IOException ex)
                {
                    throw new CoreException(ErrorCode.Codes.CannotWriteOutput, ex.Message);
                }
            }
        }
    }
}
