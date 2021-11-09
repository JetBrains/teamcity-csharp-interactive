// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMethodReturnValue.Global
namespace TeamCity.CSharpInteractive.Contracts
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public static class CommandLineExtensions
    {
        [Pure]
        public static CommandLine WithWorkingDirectory(this CommandLine commandLine, string workingDirectory) =>
            new(
                commandLine.ExecutablePath, 
                workingDirectory,
                commandLine.Args,
                commandLine.Vars);

        [Pure]
        public static CommandLine AddArgs(this CommandLine commandLine, params string[] args) =>
            new(
                commandLine.ExecutablePath, 
                commandLine.WorkingDirectory,
                new List<string>(commandLine.Args.Concat(args)).AsReadOnly(),
                commandLine.Vars);
        
        [Pure]
        public static CommandLine WithArgs(this CommandLine commandLine, params string[] args) =>
            new(
                commandLine.ExecutablePath, 
                commandLine.WorkingDirectory,
                new List<string>(args).AsReadOnly(),
                commandLine.Vars);
        
        [Pure]
        public static CommandLine AddVars(this CommandLine commandLine, params (string name, string value)[] vars) =>
            new(
                commandLine.ExecutablePath, 
                commandLine.WorkingDirectory,
                commandLine.Args,
                new List<(string, string)>(commandLine.Vars.Concat(vars)).AsReadOnly());
        
        [Pure]
        public static CommandLine WithVars(this CommandLine commandLine, params (string name, string value)[] vars) =>
            new(
                commandLine.ExecutablePath, 
                commandLine.WorkingDirectory,
                commandLine.Args,
                new List<(string, string)>(vars).AsReadOnly());
    }
}