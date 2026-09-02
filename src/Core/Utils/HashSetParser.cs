namespace Core.Utils
{
    public class HashSetParser
    {
        private static char[] validSeparators =  { ',', ';' },
            invalidChars = Path.GetInvalidFileNameChars();
        private const int maxChars = 32;
        private static string[] emptyExts = ["<empty>", "\"\""];

        public static HashSet<string> FromString(string input)
        {
            HashSet<string> result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (string.IsNullOrWhiteSpace(input))
                return result;

            string[] rawParts = input.Split(validSeparators, StringSplitOptions.TrimEntries);
            foreach (var part in rawParts)
            {
                string cleanPart = part.ToLowerInvariant();
                if (
                    string.IsNullOrWhiteSpace(cleanPart)
                    || cleanPart.Length > maxChars
                    || cleanPart.IndexOfAny(invalidChars) >= 0
                )
                    continue;
                result.Add(cleanPart);
            }
            return result;
        }

        public static HashSet<string> ToExtHashSet(HashSet<string> input)
        {
            HashSet<string> result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (input == null || input.Count == 0)
                return result;
            foreach (var part in input)
            {
                string cleanPart = part.ToLowerInvariant();
                if (emptyExts.Contains(part.ToLower()))
                {
                    result.Add("");
                    continue;
                }
                if (!cleanPart.StartsWith("."))
                    cleanPart = "." + cleanPart;
                result.Add(cleanPart);
            }
            return result;
        }
    }
}
