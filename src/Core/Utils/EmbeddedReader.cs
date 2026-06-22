using Core.Writers;

namespace Core.Utils
{
    public class EmbeddedReader
    {
        public static string ReadEmbeddedResource(string resourceName)
        {
            var assembly = typeof(HtmlWriter).Assembly;
            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException(
                        $"[AOT Error] Embedded resource '{resourceName}' is missing."
                    );
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
