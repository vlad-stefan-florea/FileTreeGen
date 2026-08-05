namespace Core.Utils
{
    public class ListParser
    {
        private static char[] validSeparators =  { ',', ';' },
            invalidChars = Path.GetInvalidFileNameChars();
        private const int maxChars = 32;
        private static string emptyExt = "<empty>";

        public static HashSet<string> ParseExtensionList(string input)
        {
            HashSet<string> result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (string.IsNullOrWhiteSpace(input))
                return result;

            string[] rawParts = input.Split(validSeparators, StringSplitOptions.TrimEntries);
            foreach (var part in rawParts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;
                if (part.Length > maxChars)
                    continue;
                if (part.IndexOfAny(invalidChars) >= 0)
                    continue;

                if (part.Equals(emptyExt, StringComparison.InvariantCultureIgnoreCase))
                // use empty quotation marks ("") to indicate a null/empty file extension
                {
                    result.Add("");
                    continue;
                }
                string cleanExt = part.ToLowerInvariant();
                if (!cleanExt.StartsWith("."))
                {
                    cleanExt = "." + cleanExt;
                }
                result.Add(cleanExt);
            }
            return result;
        }
    }
}
