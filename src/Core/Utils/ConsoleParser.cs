namespace Core.Utils
{
    public class ConsoleParser
    {
        private static char[] validSeparators = new[] { ',', ';' };

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
                if (
                    part.Equals("none", StringComparison.InvariantCultureIgnoreCase)
                    || part.Equals("\"\"", StringComparison.InvariantCultureIgnoreCase)
                ) // use ' "" ' or 'none' to indicate a null/empty file extension
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
