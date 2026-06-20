using System.Text.Json;

namespace Core.Writers
{
    public sealed class HtmlWriter : BaseWriter
    {
        readonly GenFlags _flags = new GenFlags();
        private static bool isFirstNode = true;

        public HtmlWriter(GenFlags flags)
            : base(flags)
        {
            _flags = flags;
        }

        protected override async Task WriteNodeAsync(StreamWriter writer, Node node)
        {
            if (!isFirstNode)
                await writer.WriteAsync(",");
            isFirstNode = false;
            if (!string.IsNullOrEmpty(node.Path))
                node.Path = "file:///" + node.Path.Replace("\\", "/");
            var line = JsonSerializer.Serialize(node, AppJsonContext.Default.Node);
            await writer.WriteAsync(line);
        }

        protected override async Task WriteMetadataAsync(StreamWriter writer, Metadata metadata) =>
            await writer.WriteAsync(MetadataPanel(metadata));

        protected override async Task WriteStatisticsAsync(StreamWriter writer, Statistics stats) =>
            await writer.WriteAsync(StatsPanel(stats));

        protected override async Task WriteHeaderAsync(StreamWriter writer, Metadata metadata) =>
            await writer.WriteAsync(Header(metadata));

        protected override async Task WriteFooterAsync(StreamWriter writer) =>
            await writer.WriteAsync(Footer());

        private string Footer() => "";

        private string Header(Metadata data) => "";

        private string MetadataPanel(Metadata data)
        {
            string outString = "";
            return outString;
        }

        private string StatsPanel(Statistics data)
        {
            string outString = "";
            return outString;
        }
    }
}
