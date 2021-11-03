// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
namespace TeamCity.CSharpInteractive.Contracts
{
    using System.Collections.Generic;
    using System.Linq;

    public static class CommandLineExtensions
    {
        public static CommandLine WithWorkingDirectory(this CommandLine commandLine, string workingDirectory) =>
            new(
                commandLine.ExecutablePath, 
                workingDirectory,
                commandLine.Args,
                commandLine.Vars);

        public static CommandLine AddArgs(this CommandLine commandLine, params string[] args) =>
            new(
                commandLine.ExecutablePath, 
                commandLine.WorkingDirectory,
                new List<string>(commandLine.Args.Concat(args)).AsReadOnly(),
                commandLine.Vars);
        
        public static CommandLine WithArgs(this CommandLine commandLine, params string[] args) =>
            new(
                commandLine.ExecutablePath, 
                commandLine.WorkingDirectory,
                new List<string>(args).AsReadOnly(),
                commandLine.Vars);
        
        public static CommandLine AddVars(this CommandLine commandLine, params (string name, string value)[] vars) =>
            new(
                commandLine.ExecutablePath, 
                commandLine.WorkingDirectory,
                commandLine.Args,
                new List<(string, string)>(commandLine.Vars.Concat(vars)).AsReadOnly());
        
        public static CommandLine WithVars(this CommandLine commandLine, params (string name, string value)[] vars) =>
            new(
                commandLine.ExecutablePath, 
                commandLine.WorkingDirectory,
                commandLine.Args,
                new List<(string, string)>(vars).AsReadOnly());
    }
}