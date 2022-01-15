// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Cmd;

    [Immutype.Target]
    [DebuggerTypeProxy(typeof(CommandLineResultDebugView))]
    public record CommandLineResult(IStartInfo StartInfo, int? ExitCode = default)
    {
        public override string? ToString() => StartInfo.ToString();

        private class CommandLineResultDebugView
        {
            private readonly CommandLineResult _result;

            public CommandLineResultDebugView(CommandLineResult result) => _result = result;
            
            public string ShortName => _result.StartInfo.ShortName;

            public string ExecutablePath => _result.StartInfo.ExecutablePath;

            public string WorkingDirectory => _result.StartInfo.WorkingDirectory;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<string> Args => _result.StartInfo.Args;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            private IReadOnlyList<(string name, string value)> Vars => _result.StartInfo.Vars;
                
            public int? ExitCode => _result.ExitCode;
        }
    }
}