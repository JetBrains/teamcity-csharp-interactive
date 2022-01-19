// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record Custom(
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = "",
        string WorkingDirectory = "",
        string ShortName = "")
        : IProcess
    {
        public Custom(params string[] args)
            : this(args, Enumerable.Empty<(string, string)>())
        { }
        
        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(string.IsNullOrWhiteSpace(ExecutablePath) ? host.GetService<ISettings>().DotnetExecutablePath : ExecutablePath)
                .WithShortName(string.IsNullOrWhiteSpace(ShortName) ? ((ExecutablePath == string.Empty ? "dotnet" : Path.GetFileNameWithoutExtension(ExecutablePath)) + " " + Args.FirstOrDefault()).TrimEnd() : ShortName)
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars.ToArray())
                .WithArgs(Args.ToArray());
    }
}