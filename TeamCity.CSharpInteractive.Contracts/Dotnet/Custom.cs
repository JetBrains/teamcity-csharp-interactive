// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Dotnet
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record Custom(
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        bool Integration = false,
        string ShortName = "")
        : IProcess
    {
        private readonly string _shortName = ShortName;

        public Custom()
            : this(Array.Empty<string>())
        { }
        
        public Custom(params string[] args)
            : this(args, ImmutableList<(string, string)>.Empty)
        { }
        
        public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : ExecutablePath == WellknownValues.DotnetExecutablePath ? "dotnet" : Path.GetFileNameWithoutExtension(ExecutablePath);

        public IStartInfo GetStartInfo(IHost host) =>
            new CommandLine(ExecutablePath)
                .WithWorkingDirectory(WorkingDirectory)
                .WithVars(Vars)
                .WithArgs(Args);

        public ProcessState GetState(int exitCode) => ProcessState.Unknown;
    }
}