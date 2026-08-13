using System.CommandLine;

namespace Silent
{
    public class Commands
    {
        public record Cmd(
            string Name,
            string Description,
            string? PropertyName,
            string[]? Aliases = null
        );

        // GET helpers
        static Cmd GetOption(string key) =>
            Options.TryGetValue(key, out var cmd)
                ? cmd
                : throw new ArgumentException("Option entry not found", nameof(key));

        // GENERATE helpers
        public static Option<T> GenerateOption<T>(string key, T defaultValue)
        {
            Cmd data = GetOption(key);
            var option = new Option<T>(data.Name) { Description = data.Description };
            if (defaultValue != null)
                option.DefaultValueFactory = _ => defaultValue;
            string[]? aliases = data.Aliases;
            if (aliases != null && aliases.Length > 0)
                foreach (var alias in aliases)
                    option.Aliases.Add(alias);
            return option;
        }

        static Dictionary<string, Cmd> Options = CommandData.Options,
            Arguments = CommandData.Arguments;
    }
}
