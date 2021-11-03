// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class EnvironmentVariables : IEnvironmentVariables, ITraceSource
    {
        private readonly ILog<EnvironmentVariables> _log;
        private readonly IStringService _stringService;

        public EnvironmentVariables(ILog<EnvironmentVariables> log, IStringService stringService)
        {
            _log = log;
            _stringService = stringService;
        }

        public string? GetEnvironmentVariable(string variable)
        {
            var value = System.Environment.GetEnvironmentVariable(variable);
            _log.Trace($"Get environment variable {variable} = \"{value}\".");
            return value;
        }

        public IEnumerable<Text> GetTrace()
        {
            yield return new Text("Environment variables:");
            foreach (var entry in System.Environment.GetEnvironmentVariables().OfType<DictionaryEntry>().OrderBy(i => i.Key))
            {
                yield return new Text($"{_stringService.Tab}{entry.Key}={entry.Value}");
            }
        }
    }
}