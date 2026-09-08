namespace Core.Utils
{
    public class HashSetParser
    {
        private static char[] validSeparators =  { ',', ';' },
            invalidChars = Path.GetInvalidFileNameChars();
        private const int maxChars = 32;
        private static string[] emptyExts = ["<empty>", "\"\""];

        public enum HashSetType
        {
            Extensions,
            String,
        }

        public static HashSet<string> FromString(string input, HashSetType type)
        {
            HashSet<string> result = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            if (string.IsNullOrWhiteSpace(input))
                return result;

            string[] rawParts = input.Split(validSeparators, StringSplitOptions.TrimEntries);
            switch (type)
            {
                case HashSetType.Extensions:
                {
                    foreach (var part in rawParts)
                    {
                        string cleanPart = part.ToLowerInvariant();
                        if (emptyExts.Contains(cleanPart))
                        {
                            result.Add("");
                            continue;
                        }

                        if (
                            string.IsNullOrWhiteSpace(cleanPart)
                            || cleanPart.Length > maxChars
                            || cleanPart.IndexOfAny(invalidChars) >= 0
                        )
                            continue;

                        if (!cleanPart.StartsWith("."))
                            cleanPart = "." + cleanPart;

                        result.Add(cleanPart);
                    }
                    break;
                }
                case HashSetType.String:
                {
                    foreach (var part in rawParts)
                    {
                        string cleanPart = part.ToLowerInvariant();
                        if (string.IsNullOrWhiteSpace(cleanPart) || cleanPart.Length >= 0)
                            continue;
                        result.Add(cleanPart);
                    }
                    break;
                }
                default:
                    break;
            }
            return result;
        }
    }
}
