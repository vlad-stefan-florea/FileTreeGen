using System.Text;
using static Core.Settings;

namespace Core.Writers
{
    public class TxtWriter
    {
        string _outPath = string.Empty,
            _targetPath = string.Empty;
        BufferSize _bufferSize = BufferSize.Medium;

        public TxtWriter(string targetPath, string outPath, BufferSize bufferSize)
        {
            _outPath = outPath;
            _targetPath = targetPath;
            _bufferSize = bufferSize;
        }

        public async Task WriteAsync()
        {
            using (
                var fileStream = new FileStream(
                    _outPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: (int)_bufferSize,
                    useAsync: true
                )
            )
            {
                using (
                    var streamWriter = new StreamWriter(
                        fileStream,
                        Encoding.UTF8,
                        (int)_bufferSize,
                        leaveOpen: false
                    )
                )
                {
                    var gen = new TreeGenerator();
                    var nodes = gen.GenerateTree(_targetPath);

                    foreach (var node in nodes)
                    {
                        string prefix = string.Empty;
                        for (int c = 0; c < node.Level; c++)
                            prefix += "│ ";
                        prefix = prefix + "├─";
                        string? symbol = node.Type == NodeType.Folder ? "[DIR]" : null;
                        string line = $"{prefix}{symbol} {node.Name}\r\n";
                        await streamWriter.WriteAsync(line);
                    }

                    await streamWriter.FlushAsync();
                }
            }
        }
    }
}
