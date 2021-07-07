// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using Host;
    using NuGet.Versioning;

    internal class AddPackageReferenceCommandFactory: ICommandFactory<string>
    {
        private readonly ILog<AddPackageReferenceCommandFactory> _log;

        public AddPackageReferenceCommandFactory(ILog<AddPackageReferenceCommandFactory> log) => _log = log;

        public IEnumerable<ICommand> Create(string replCommand)
        {
            replCommand = replCommand.Trim();
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
            if (parts.Length is < 1 or > 2)
            {
                _log.Error(ErrorId.InvalidScriptDirective, $"Invalid script directive \"{replCommand}\".");
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
                else
                {
                    _log.Error(ErrorId.CannotParsePackageVersion, $"Cannot parse the package version \"{parts[1]}\".");
                    yield break;
                }
            }
                
            _log.Trace(new []{new Text($"REPL #r {rawId} {version}")});
            yield return new AddPackageReferenceCommand(rawId, version);
        }
    }
}