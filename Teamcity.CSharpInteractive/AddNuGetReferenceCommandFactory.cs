// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using NuGet.Versioning;

    internal class AddNuGetReferenceCommandFactory: ICommandFactory<string>
    {
        private static readonly Regex NuGetReferenceRegex = new(@"^\s*#r\s+""nuget:\s*([^,\s]+?)(,\s*([^\s]+?)\s*|\s*)""\s*$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<AddNuGetReferenceCommandFactory> _log;

        public AddNuGetReferenceCommandFactory(ILog<AddNuGetReferenceCommandFactory> log) =>
            _log = log;

        public int Order => 0;

        public IEnumerable<ICommand> Create(string replCommand)
        {
            var match = NuGetReferenceRegex.Match(replCommand);
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
                
            _log.Trace(new []{new Text($"REPL #r \"nuget:{packageIdStr}, {version}\"")});
            yield return new AddNuGetReferenceCommand(packageIdStr, version);
        }
    }
}