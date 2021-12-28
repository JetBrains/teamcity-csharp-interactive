// ReSharper disable CheckNamespace
namespace Cmd
{
    using System.Collections.Generic;

    public interface IStartInfo
    {
        string ShortName { get; }
        
        string ExecutablePath { get; }

        string WorkingDirectory { get; }

        IReadOnlyList<string> Args { get; }

        IReadOnlyList<(string name, string value)> Vars { get; }
    }
}