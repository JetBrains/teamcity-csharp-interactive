// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
namespace Docker
{
    using System.Collections.Generic;
    using System.Linq;
    using Cmd;

    internal static class CommandLineExtensions
    {
        public static CommandLine AddBooleanArgs(this CommandLine cmd, params (string name, bool? value)[] args) =>
            cmd.AddArgs((
                from arg in args
                where arg.value ?? false
                select arg.name)
                .ToArray());
        
        public static CommandLine AddArgs(this CommandLine cmd, params (string name, string value)[] args) =>
            cmd.AddArgs((
                from arg in args
                where !string.IsNullOrWhiteSpace(arg.value)
                select new[] {arg.name, arg.value})
                .SelectMany(i => i)
                .ToArray());
        
        public static CommandLine AddArgs(this CommandLine cmd, string name, IEnumerable<string> values) =>
            cmd.AddArgs((
                    from val in values
                    where !string.IsNullOrWhiteSpace(val)
                    select new[] { name, val})
                .SelectMany(i => i)
                .ToArray());
        
        public static CommandLine AddValues(this CommandLine cmd, string name, string delimiter, params (string key, string value)[] values) =>
            cmd.AddArgs((
                from val in values
                where !string.IsNullOrWhiteSpace(val.key) && !string.IsNullOrWhiteSpace(val.value)
                select new[] { name, $"{val.key}{delimiter}{val.value}"})
                .SelectMany(i => i)
                .ToArray());
    }
}