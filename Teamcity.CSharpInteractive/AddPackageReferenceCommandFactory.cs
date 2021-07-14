// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using NuGet.Versioning;

    internal class AddPackageReferenceCommandFactory: ICommandFactory<string>
    {
        private static readonly Regex Regex = new(@"^\s*#r\s+""nuget:\s*([^,\s]+?)(,\s*([^\s]+?)\s*|\s*)""\s*$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<AddPackageReferenceCommandFactory> _log;

        public AddPackageReferenceCommandFactory(ILog<AddPackageReferenceCommandFactory> log) => _log = log;

        public IEnumerable<ICommand> Create(string replCommand)
        {
            var match = Regex.Match(replCommand);
            if (!match.Success)
            {
                yield break;
            }

            var packageIdStr = match.Groups[1].Value;
            var versionStr = match.Groups[3].Value;
            
            NuGetVersion? version = null;
            if (!string.IsNullOrWhiteSpace(versionStr))
            {
                if (NuGetVersion.TryParse(versionStr, out var curVersion))
                {
                    version = curVersion;
                }
                else
                {
                    _log.Error(ErrorId.CannotParsePackageVersion, $"Cannot parse the package version \"{versionStr}\".");
                    yield break;
                }
            }
                
            _log.Trace(new []{new Text($"REPL #r {packageIdStr} {version}")});
            yield return new AddPackageReferenceCommand(packageIdStr, version);
        }
    }
}