// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Dotnet
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Cmd;
    using TeamCity.CSharpInteractive.Contracts;

    [Immutype.Target]
    public record Custom(
        IEnumerable<string> Args,
        IEnumerable<(string name, string value)> Vars,
        string ExecutablePath = WellknownValues.DotnetExecutablePath,
        string WorkingDirectory = "",
        bool Integration = false)
    {
        public Custom()
            : this(Array.Empty<string>())
        { }
        
        public Custom(params string[] args)
            : this(args, ImmutableList<(string, string)>.Empty)
        { }

        public static implicit operator CommandLine(Custom it) =>
            new CommandLine(it.ExecutablePath)
            .WithWorkingDirectory(it.WorkingDirectory)
            .WithVars(it.Vars)
            .WithArgs(it.Args);
    }
}