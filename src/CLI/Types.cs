using Core;

namespace CLI
{
    internal class Types
    {
        public record Command(
            string Name,
            string Description,
            string Syntax,
            List<Argument>? Arguments = null,
            List<Command>? Subcommands = null,
            List<Option>? Options = null
        );

        public record Argument(string Placeholder, string Description);

        public record Option(
            string Name,
            string Description,
            string Syntax,
            string FlagName,
            string[]? Aliases = null
        );

        // GENERATE helpers
        public static Type? GetFlagType(string flagName)
        {
            return typeof(GenFlags).GetProperty(flagName)?.PropertyType;
        }
    }
}
