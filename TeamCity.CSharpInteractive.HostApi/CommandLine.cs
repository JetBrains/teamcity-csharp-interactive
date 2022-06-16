// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Global
namespace HostApi;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Immutype;

/// <summary>
/// Runs an arbitrary executable with arguments and environment variables from the working directory.
/// </summary>
[Target]
[DebuggerTypeProxy(typeof(CommandLineDebugView))]
public partial record CommandLine(
    // Specifies the application executable path.
    string ExecutablePath,
    // Specifies the working directory for the application to be started.
    string WorkingDirectory,
    // Specifies the set of command line arguments to use when starting the application.
    IEnumerable<string> Args,
    // Specifies the set of environment variables that apply to this process and its child processes.
    IEnumerable<(string name, string value)> Vars,
    // Specifies a short name for this command line.
    string ShortName = "")
    : IStartInfo
{
    private readonly string _shortName = ShortName;

    public CommandLine(string executablePath, params string[] args)
        : this(executablePath, string.Empty, args, ImmutableArray<(string name, string value)>.Empty)
    { }

    internal CommandLine(IStartInfo startInfo)
        : this(startInfo.ExecutablePath, startInfo.WorkingDirectory, startInfo.Args, startInfo.Vars, startInfo.ShortName)
    { }

    public string ShortName => !string.IsNullOrWhiteSpace(_shortName) ? _shortName : Path.GetFileNameWithoutExtension(ExecutablePath);

    public IStartInfo GetStartInfo(IHost host) => this;
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(ShortName))
        {
            sb.Append(ShortName);
            sb.Append(": ");
        }

        sb.Append(Escape(ExecutablePath));
        foreach (var arg in Args)
        {
            sb.Append(' ');
            sb.Append(Escape(arg));
        }

        return sb.ToString();
    }

    private static string Escape(string text) => !text.TrimStart().StartsWith("\"") && text.Contains(' ') ? $"\"{text}\"" : text;

    internal class CommandLineDebugView
    {
        private readonly IStartInfo _startInfo;

        public CommandLineDebugView(IStartInfo startInfo) => _startInfo = startInfo;

        public string ShortName => _startInfo.ShortName;

        public string ExecutablePath => _startInfo.ExecutablePath;

        public string WorkingDirectory => _startInfo.WorkingDirectory;

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public IEnumerable<string> Args => _startInfo.Args;

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public IEnumerable<(string name, string value)> Vars => _startInfo.Vars;
    }
}