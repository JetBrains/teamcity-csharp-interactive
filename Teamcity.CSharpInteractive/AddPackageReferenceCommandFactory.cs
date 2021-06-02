// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using NuGet.Versioning;

    internal class AddPackageReferenceCommandFactory: IReplCommandFactory
    {
        private readonly ILog<AddPackageReferenceCommandFactory> _log;

        public AddPackageReferenceCommandFactory(ILog<AddPackageReferenceCommandFactory> log) => _log = log;

        public IEnumerable<ICommand> TryCreate(string replCommand)
        {
            if (!replCommand.StartsWith("#r", StringComparison.CurrentCultureIgnoreCase))
            {
                yield break;
            }

            replCommand = replCommand[2..].Trim();
            if (replCommand.StartsWith("\""))
            {
                yield break;
            }

            var parts = replCommand.Split( ' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                yield break;
            }
            
            var rawId = parts[0];
            NuGetVersion? version = null;
            if (parts.Length >= 2)
            {
                if (NuGetVersion.TryParse(parts[1], out var curVersion))
                {
                    version = curVersion;
                }
            }
                
            _log.Trace(new []{new Text($"REPL #r {rawId} {version}")});
            yield return new AddPackageReferenceCommand(rawId, version);
        }
    }
}