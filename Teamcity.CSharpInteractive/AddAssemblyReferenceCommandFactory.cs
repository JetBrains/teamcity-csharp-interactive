// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal class AddAssemblyReferenceCommandFactory: ICommandFactory<string>
    {
        private static readonly Regex LibReferenceRegex = new(@"^\s*#r\s+""\s*(.+?)""\s*$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<AddAssemblyReferenceCommandFactory> _log;
        private readonly IAssemblyPathResolver _assemblyPathResolver;

        public AddAssemblyReferenceCommandFactory(
            ILog<AddAssemblyReferenceCommandFactory> log,
            IAssemblyPathResolver assemblyPathResolver)
        {
            _log = log;
            _assemblyPathResolver = assemblyPathResolver;
        }

        public int Order => 1;

        public IEnumerable<ICommand> Create(string replCommand)
        {
            var match = LibReferenceRegex.Match(replCommand);
            if (match.Success)
            {
                var assemblyPath = match.Groups[1].Value;
                if (_assemblyPathResolver.TryResolve(assemblyPath, out var fullAssemblyPath))
                {
                    _log.Trace(new []{new Text($"REPL #r \"{fullAssemblyPath}\"")});
                    yield return new ScriptCommand(assemblyPath, $"#r \"{fullAssemblyPath}\"");
                }
            }
        }
    }
}