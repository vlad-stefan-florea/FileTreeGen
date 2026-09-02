using System.CommandLine;
using Core;

namespace CLI
{
    public class Commands
    {
        public record Cmd(
            string Name,
            string Description,
            string FlagName,
            string[]? Aliases = null
        );

        public record SubCmd(string Name, string Description, string? UsageArgs = null);

        // GENERATE helpers
        public static Option<T> NewScanOption<T>(Cmd data, T defaultValue)
        {
            var option = new Option<T>(data.Name) { Description = data.Description };
            if (defaultValue != null)
                option.DefaultValueFactory = _ => defaultValue;
            string[]? aliases = data.Aliases;
            if (aliases != null && aliases.Length > 0)
                foreach (var alias in aliases)
                    option.Aliases.Add(alias);
            return option;
        }

        public static Command GenerateSubcmd(SubCmd data) =>
            new Command(data.Name, data.Description);

        public static Type? GetFlagType(string flagName)
        {
            return typeof(GenFlags).GetProperty(flagName)?.PropertyType;
        }
    }
}
